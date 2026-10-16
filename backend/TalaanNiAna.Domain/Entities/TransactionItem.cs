public class TransactionItem : IHasStore
{
    public int Id { get; set; }

    /// <summary>
    /// Denormalised from the parent transaction. Reporting queries read this
    /// table directly, and a filter on a navigation would not cover them.
    /// </summary>
    public int StoreId { get; set; }

    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Cost at the moment of sale. Snapshotted so that changing a product's
    /// cost later does not rewrite the profit on sales already made.
    /// </summary>
    public decimal UnitCost { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
    public decimal Profit => Quantity * (UnitPrice - UnitCost);
}
