namespace TalaanNiAna.Tests.Fakes;

/// <summary>
/// Records what the services wrote. Stateful, because the tests assert on the
/// ledger left behind rather than on which calls were made.
/// </summary>
public sealed class FakeStockMovementRepository : IStockMovementRepository
{
    public List<StockMovement> Movements { get; } = [];

    public void Add(StockMovement movement) => Movements.Add(movement);

    public Task RecordAsync(StockMovement movement)
    {
        Movements.Add(movement);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<StockMovement>> GetForProductAsync(int productId, int count = 50)
        => Task.FromResult(Movements
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.OccurredAt)
            .Take(count));

    public Task<IEnumerable<StockMovement>> GetShrinkageAsync(DateRange range)
        => Task.FromResult(Movements
            .Where(m => m.OccurredAt >= range.FromUtc && m.OccurredAt < range.ToUtc)
            .Where(m => m.Quantity < 0
                     && m.Reason is StockReason.Spoilage or StockReason.Adjustment));
}
