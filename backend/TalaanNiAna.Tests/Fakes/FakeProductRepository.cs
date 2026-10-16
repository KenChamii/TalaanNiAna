namespace TalaanNiAna.Tests.Fakes;

/// <summary>
/// In-memory products. Stateful on purpose: checkout tests assert on the
/// stock left behind, which a mock's call verification would not show.
/// </summary>
public sealed class FakeProductRepository : IProductRepository
{
    private readonly Dictionary<int, Product> _products = [];

    public Product Add(int id, string name, decimal price, int stock, decimal cost = 0m)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Price = price,
            CostPrice = cost,
            StockQuantity = stock,
            Category = "Test",
            IsActive = true
        };

        _products[id] = product;
        return product;
    }

    public Product this[int id] => _products[id];

    public Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        IReadOnlyDictionary<int, Product> found = ids
            .Distinct()
            .Where(_products.ContainsKey)
            .ToDictionary(id => id, id => _products[id]);

        return Task.FromResult(found);
    }

    public Task<Product?> GetByIdAsync(int id)
        => Task.FromResult(_products.GetValueOrDefault(id));

    // Not exercised by these tests.
    public Task<IEnumerable<Product>> GetAllAsync(string? search, string? category, int page, int pageSize)
        => throw new NotSupportedException();
    public Task<IEnumerable<string>> GetCategoriesAsync() => throw new NotSupportedException();
    public Task<Product> CreateAsync(Product product) => throw new NotSupportedException();
    public Task<Product?> UpdateAsync(Product product) => throw new NotSupportedException();
    public Task<bool> DeleteAsync(int id) => throw new NotSupportedException();
    public Task<IEnumerable<Product>> GetLowStockAsync() => throw new NotSupportedException();
    public Task<int> GetLowStockCountAsync() => throw new NotSupportedException();
    public Task<int> GetTotalCountAsync(string? search, string? category) => throw new NotSupportedException();
    public Task<IEnumerable<Product>> GetQuickItemsAsync(int count = 24) => throw new NotSupportedException();
}
