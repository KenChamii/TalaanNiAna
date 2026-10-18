using Microsoft.EntityFrameworkCore;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _context;

    public StockMovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(StockMovement movement) => _context.StockMovements.Add(movement);

    public async Task RecordAsync(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockMovement>> GetForProductAsync(int productId, int count = 50)
        => await _context.StockMovements
            .AsNoTracking()
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.OccurredAt)
            .ThenByDescending(m => m.Id)   // same-second movements still read in order
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<StockMovement>> GetShrinkageAsync(DateRange range)
        => await _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .Where(m => m.OccurredAt >= range.FromUtc && m.OccurredAt < range.ToUtc)
            .Where(m => m.Quantity < 0
                     && (m.Reason == StockReason.Spoilage || m.Reason == StockReason.Adjustment))
            .OrderByDescending(m => m.OccurredAt)
            .ToListAsync();
}
