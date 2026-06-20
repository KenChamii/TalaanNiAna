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
    {
        var query = _context.Products.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        return await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

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
        existing.Price = product.Price;
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

        // Soft delete — keeps historical TransactionItem rows valid
        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
        => await _context.Products
            .Where(p => p.IsActive && p.StockQuantity <= threshold)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync(string? search, string? category)
    {
        var query = _context.Products.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Product>> GetQuickItemsAsync()
        // "Quick items" = the tiles shown on the POS screen (Page 5).
        // Simplest useful rule: the 8 most recently sold / most stocked active items.
        // Swap this for a `IsQuickItem` flag on Product later if Nanay Ana wants to pick them manually.
        => await _context.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.StockQuantity)
            .Take(8)
            .ToListAsync();
}