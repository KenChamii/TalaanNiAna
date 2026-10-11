public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? search, string? category, int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);

    /// <summary>Loads several products in one query. Used by checkout to avoid N+1.</summary>
    Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids);

    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
    Task<int> GetTotalCountAsync(string? search, string? category);
    Task<IEnumerable<Product>> GetQuickItemsAsync(int count = 24);
}
