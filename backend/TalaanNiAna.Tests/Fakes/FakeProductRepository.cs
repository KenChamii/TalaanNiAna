namespace TalaanNiAna.Tests.Fakes;

/// <summary>
/// In-memory products. Stateful on purpose: checkout tests assert on the
/// stock left behind, which a mock's call verification would not show.
/// </summary>
public sealed class FakeProductRepository : IProductRepository
{
    private readonly Dictionary<int, Product> _products = [];

    public Product Add(
        int id, string name, decimal price, int stock, decimal cost = 0m, string? barcode = null)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Price = price,
            CostPrice = cost,
            Barcode = barcode,
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

    public Task<Product?> GetByBarcodeAsync(string barcode)
        => Task.FromResult(_products.Values
            .FirstOrDefault(p => p.IsActive && p.Barcode == barcode));

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _products.Count == 0 ? 1 : _products.Keys.Max() + 1;
        _products[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(Product product)
    {
        if (!_products.TryGetValue(product.Id, out var existing))
            return Task.FromResult<Product?>(null);

        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Barcode = product.Barcode;
        existing.Price = product.Price;
        existing.CostPrice = product.CostPrice;
        existing.StockQuantity = product.StockQuantity;
        existing.LowStockThreshold = product.LowStockThreshold;

        return Task.FromResult<Product?>(existing);
    }

    public Task<bool> DeleteAsync(int id)
    {
        if (!_products.TryGetValue(id, out var existing)) return Task.FromResult(false);

        existing.IsActive = false;
        return Task.FromResult(true);
    }

    // Not exercised by these tests.
    public Task<IEnumerable<Product>> GetAllAsync(string? search, string? category, int page, int pageSize)
        => throw new NotSupportedException();
    public Task<IEnumerable<string>> GetCategoriesAsync() => throw new NotSupportedException();
    public Task<IEnumerable<Product>> GetLowStockAsync() => throw new NotSupportedException();
    public Task<int> GetLowStockCountAsync() => throw new NotSupportedException();
    public Task<int> GetTotalCountAsync(string? search, string? category) => throw new NotSupportedException();
    public Task<IEnumerable<Product>> GetQuickItemsAsync(int count = 24) => throw new NotSupportedException();
}
