public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? search, string? category, int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
    Task<int> GetTotalCountAsync(string? search, string? category);
    Task<IEnumerable<Product>> GetQuickItemsAsync();
}