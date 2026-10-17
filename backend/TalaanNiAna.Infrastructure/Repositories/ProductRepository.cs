using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(
        string? search, string? category, int page, int pageSize)
        => await Filter(search, category)
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<Product?> GetByBarcodeAsync(string barcode)
        => await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IsActive && p.Barcode == barcode);

    public async Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var distinct = ids.Distinct().ToArray();

        // Tracked on purpose: checkout mutates these and relies on the concurrency token.
        return await _context.Products
            .Where(p => p.IsActive && distinct.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
        => await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.Category != "")
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var existing = await _context.Products.FindAsync(product.Id);
        if (existing is null) return null;

        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Barcode = product.Barcode;
        existing.Price = product.Price;
        existing.CostPrice = product.CostPrice;
        existing.StockQuantity = product.StockQuantity;
        existing.LowStockThreshold = product.LowStockThreshold;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing is null) return false;

        // Soft delete keeps historical TransactionItem rows valid.
        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync()
        => await LowStock()
            .AsNoTracking()
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();

    public async Task<int> GetLowStockCountAsync()
        => await LowStock().CountAsync();

    public async Task<int> GetTotalCountAsync(string? search, string? category)
        => await Filter(search, category).CountAsync();

    public async Task<IEnumerable<Product>> GetQuickItemsAsync(int count = 24)
        => await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.StockQuantity)
            .Take(count)
            .ToListAsync();

    /// <summary>Per-product threshold. Store supplies the default for new products.</summary>
    private IQueryable<Product> LowStock()
        => _context.Products.Where(p => p.IsActive && p.StockQuantity <= p.LowStockThreshold);

    private IQueryable<Product> Filter(string? search, string? category)
    {
        var query = _context.Products.Where(p => p.IsActive);

        // Typing a code into the search box finds the item, for when the
        // scanner is unplugged and the owner reads the number off the pack.
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Barcode == search);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        return query;
    }
}
