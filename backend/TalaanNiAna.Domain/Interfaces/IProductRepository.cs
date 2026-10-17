public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? search, string? category, int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);

    /// <summary>Exact match on a scanned code. Null when nothing carries it.</summary>
    Task<Product?> GetByBarcodeAsync(string barcode);

    /// <summary>Loads several products in one query. Used by checkout to avoid N+1.</summary>
    Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids);

    /// <summary>Distinct categories in use by this store, alphabetically.</summary>
    Task<IEnumerable<string>> GetCategoriesAsync();

    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);

    /// <summary>Products at or below their own LowStockThreshold.</summary>
    Task<IEnumerable<Product>> GetLowStockAsync();
    Task<int> GetLowStockCountAsync();

    Task<int> GetTotalCountAsync(string? search, string? category);
    Task<IEnumerable<Product>> GetQuickItemsAsync(int count = 24);
}
