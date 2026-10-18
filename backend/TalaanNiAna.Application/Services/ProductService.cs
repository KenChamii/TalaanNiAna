using TalaanNiAna.Application.Common.Exceptions;

public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IStoreRepository _stores;
    private readonly IStockMovementRepository _movements;

    public ProductService(
        IProductRepository repo,
        IStoreRepository stores,
        IStockMovementRepository movements)
    {
        _repo = repo;
        _stores = stores;
        _movements = movements;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search, string? category, int page = 1, int pageSize = 10)
    {
        var products = await _repo.GetAllAsync(search, category, page, pageSize);
        var total = await _repo.GetTotalCountAsync(search, category);

        return new PagedResult<ProductDto>
        {
            Items = products.Select(ToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : ToDto(product);
    }

    /// <summary>Scanner lookup. Null when no active product carries the code.</summary>
    public async Task<ProductDto?> GetByBarcodeAsync(string barcode)
    {
        var normalized = NormalizeBarcode(barcode);
        if (normalized is null) return null;

        var product = await _repo.GetByBarcodeAsync(normalized);
        return product is null ? null : ToDto(product);
    }

    /// <summary>Categories actually in use, for the inventory filter and form.</summary>
    public async Task<IEnumerable<string>> GetCategoriesAsync()
        => await _repo.GetCategoriesAsync();

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // No threshold given means "use the store default", not "never warn me".
        var threshold = dto.LowStockThreshold ?? (await _stores.GetCurrentAsync()).LowStockAlertLimit;
        var barcode = NormalizeBarcode(dto.Barcode);

        await EnsureBarcodeIsFree(barcode, productId: null);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Barcode = barcode,
            Price = dto.Price,
            CostPrice = dto.CostPrice,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = threshold,
            ParentProductId = dto.ParentProductId,
            UnitsPerParent = dto.UnitsPerParent
        };

        await EnsureTingiLinkIsSound(product, productId: null);

        var created = await _repo.CreateAsync(product);

        // Opening count, so the ledger explains the stock a product starts with
        // rather than beginning with an unexplained number.
        if (created.StockQuantity != 0)
        {
            await _movements.RecordAsync(new StockMovement
            {
                ProductId = created.Id,
                Quantity = created.StockQuantity,
                Reason = StockReason.Opening,
                BalanceAfter = created.StockQuantity
            });
        }

        return ToDto(created);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var existing = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Produkto", id);

        // Null keeps what is there; an empty string is the form clearing the code.
        var barcode = dto.Barcode is null ? existing.Barcode : NormalizeBarcode(dto.Barcode);

        await EnsureBarcodeIsFree(barcode, productId: id);

        var product = new Product
        {
            Id = id,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Barcode = barcode,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            // Omitted values must not silently reset what the store already set.
            CostPrice = dto.CostPrice ?? existing.CostPrice,
            LowStockThreshold = dto.LowStockThreshold ?? existing.LowStockThreshold,
            // Zero is the form clearing the link; null leaves it as it was.
            ParentProductId = dto.ParentProductId switch
            {
                null => existing.ParentProductId,
                0 => null,
                var value => value
            },
            UnitsPerParent = dto.ParentProductId == 0
                ? null
                : dto.UnitsPerParent ?? existing.UnitsPerParent
        };

        await EnsureTingiLinkIsSound(product, productId: id);

        // Captured before the update, so the movement records the size of the change.
        var stockDelta = dto.StockQuantity - existing.StockQuantity;

        var updated = await _repo.UpdateAsync(product);
        if (updated is null) return null;

        // Editing the stock field is a correction, not a sale. Recorded as one.
        if (stockDelta != 0)
        {
            await _movements.RecordAsync(new StockMovement
            {
                ProductId = updated.Id,
                Quantity = stockDelta,
                Reason = StockReason.Adjustment,
                BalanceAfter = updated.StockQuantity,
                Note = "Binago sa form ng imbentaryo"
            });
        }

        return ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<IEnumerable<QuickItemDto>> GetQuickItemsAsync()
    {
        var items = await _repo.GetQuickItemsAsync();
        return items.Select(p => new QuickItemDto(p.Id, p.Name, p.Price));
    }

    /// <summary>
    /// A tingi line needs a pack to come out of and a count of what it yields.
    /// Both or neither: half a link would leave repacking unable to run and no
    /// obvious reason why.
    /// </summary>
    private async Task EnsureTingiLinkIsSound(Product product, int? productId)
    {
        if (product.ParentProductId is null && product.UnitsPerParent is null) return;

        if (product.ParentProductId is null)
            throw new RequestValidationException(nameof(product.ParentProductId),
                "Piliin kung saang pakete galing.");

        if (product.UnitsPerParent is null or < 2)
            throw new RequestValidationException(nameof(product.UnitsPerParent),
                "Ilan ang lalabas sa isang pakete? Dapat dalawa pataas.");

        if (product.ParentProductId == productId)
            throw new RequestValidationException(nameof(product.ParentProductId),
                "Hindi pwedeng ang sarili nito ang pinagmulan.");

        var parent = await _repo.GetByIdAsync(product.ParentProductId.Value)
            ?? throw new NotFoundException("Produkto", product.ParentProductId.Value);

        // One level only. A sack to a cup is a repack; a sack to a cup to a
        // spoonful is a supply chain, and nothing here is built for it.
        if (parent.ParentProductId is not null)
            throw new RequestValidationException(nameof(product.ParentProductId),
                $"Tingi na rin ang {parent.Name}. Isang antas lang ang pwede.");
    }

    /// <summary>
    /// Checked here so the owner gets a readable message naming the other item,
    /// rather than the unique index error the database would raise.
    /// </summary>
    private async Task EnsureBarcodeIsFree(string? barcode, int? productId)
    {
        if (barcode is null) return;

        var owner = await _repo.GetByBarcodeAsync(barcode);
        if (owner is null || owner.Id == productId) return;

        throw new ConflictException(
            $"Ginagamit na ang barcode na ito ng {owner.Name}.", "duplicate_barcode");
    }

    /// <summary>Trims and upper-cases so the same pack always scans to one product.</summary>
    private static string? NormalizeBarcode(string? barcode)
    {
        var trimmed = barcode?.Trim().ToUpperInvariant();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Category, p.Barcode,
        p.Price, p.CostPrice, p.UnitMargin, MarginPercent(p),
        p.StockQuantity, p.LowStockThreshold,
        p.StockQuantity <= p.LowStockThreshold,
        p.ParentProductId, p.ParentProduct?.Name, p.UnitsPerParent);

    /// <summary>Margin over price. Zero when no cost has been recorded yet.</summary>
    private static decimal MarginPercent(Product p)
        => p.Price <= 0 || p.CostPrice <= 0
            ? 0
            : Math.Round(p.UnitMargin / p.Price * 100, 1);
}

public record QuickItemDto(int Id, string Name, decimal Price);
