/// <summary>
/// Why a product's stock changed. Without this, a shortfall at stock-take is
/// indistinguishable from spoilage, theft or a miscount, and the only honest
/// thing the owner can do is overwrite the number and move on.
/// </summary>
public class StockMovement : IHasStore
{
    public int Id { get; set; }
    public int StoreId { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Signed. Negative takes stock out, positive puts it back. Kept signed
    /// rather than paired with a direction flag so a sum over the column is
    /// the balance, with nothing to get the wrong way round.
    /// </summary>
    public int Quantity { get; set; }

    public StockReason Reason { get; set; }

    /// <summary>
    /// Stock on hand once this movement was applied. Denormalised so the
    /// history reads without re-summing every earlier row, and so a later
    /// correction cannot silently rewrite what the owner saw at the time.
    /// </summary>
    public int BalanceAfter { get; set; }

    /// <summary>The sale this came from, when it came from one.</summary>
    public int? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    /// <summary>What the person typed. Required for anything they chose to record.</summary>
    public string? Note { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public enum StockReason
{
    /// <summary>Stock counted in when the product was first added.</summary>
    Opening = 0,

    Sale = 1,

    /// <summary>Delivery from a supplier.</summary>
    Restock = 2,

    /// <summary>A count that disagreed with the system, corrected either way.</summary>
    Adjustment = 3,

    /// <summary>Expired, broken, or eaten by the store cat.</summary>
    Spoilage = 4,

    /// <summary>Customer brought it back.</summary>
    Return = 5,

    /// <summary>A sack or box broken down into tingi. Pairs with a Repack in.</summary>
    RepackOut = 6,
    RepackIn = 7
}
