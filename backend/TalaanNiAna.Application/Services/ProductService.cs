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

    /// <summary>Categories actually in use, for the inventory filter and form.</summary>
    public async Task<IEnumerable<string>> GetCategoriesAsync()
        => await _repo.GetCategoriesAsync();

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // No threshold given means "use the store default", not "never warn me".
        var threshold = dto.LowStockThreshold ?? (await _stores.GetCurrentAsync()).LowStockAlertLimit;

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
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

        var product = new Product
        {
            Id = id,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
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

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Category,
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
