using TalaanNiAna.Application.Common.Exceptions;

public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IStoreRepository _stores;

    public ProductService(IProductRepository repo, IStoreRepository stores)
    {
        _repo = repo;
        _stores = stores;
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
            LowStockThreshold = threshold
        };

        var created = await _repo.CreateAsync(product);
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
            LowStockThreshold = dto.LowStockThreshold ?? existing.LowStockThreshold
        };

        var updated = await _repo.UpdateAsync(product);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<IEnumerable<QuickItemDto>> GetQuickItemsAsync()
    {
        var items = await _repo.GetQuickItemsAsync();
        return items.Select(p => new QuickItemDto(p.Id, p.Name, p.Price));
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
        p.StockQuantity <= p.LowStockThreshold);

    /// <summary>Margin over price. Zero when no cost has been recorded yet.</summary>
    private static decimal MarginPercent(Product p)
        => p.Price <= 0 || p.CostPrice <= 0
            ? 0
            : Math.Round(p.UnitMargin / p.Price * 100, 1);
}

public record QuickItemDto(int Id, string Name, decimal Price);
