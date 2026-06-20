public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
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

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = dto.LowStockThreshold
        };

        var created = await _repo.CreateAsync(product);
        return ToDto(created);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = dto.LowStockThreshold
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

    // Maps Product -> ProductDto. Field order here must match ProductDto's
    // positional record parameters exactly: Id, Name, Category, Price, StockQuantity, IsLowStock.
    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Category, p.Price, p.StockQuantity,
        p.StockQuantity <= p.LowStockThreshold);
}

public record QuickItemDto(int Id, string Name, decimal Price);