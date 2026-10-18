public interface IStockMovementRepository
{
    /// <summary>
    /// Queues a movement without saving. For callers that already own a
    /// transaction, so a sale and its movements commit together or not at all.
    /// </summary>
    void Add(StockMovement movement);

    /// <summary>
    /// Writes a movement immediately. For callers with no transaction of their
    /// own, such as adding a product or correcting its stock on the form.
    /// </summary>
    Task RecordAsync(StockMovement movement);

    /// <summary>Newest first, for one product's history screen.</summary>
    Task<IEnumerable<StockMovement>> GetForProductAsync(int productId, int count = 50);

    /// <summary>
    /// Everything that left the shelf without being sold, for the window given.
    /// Spoilage and negative adjustments are the shrinkage the owner is paying for.
    /// </summary>
    Task<IEnumerable<StockMovement>> GetShrinkageAsync(DateRange range);
}
