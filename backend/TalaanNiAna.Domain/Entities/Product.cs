public class Product : IHasStore
{
    public int Id { get; set; }
    public int StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Scanned code, usually EAN-13 on packaged goods. Null for loose items
    /// sold by the piece, which have nothing printed on them.
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>What the store sells it for.</summary>
    public decimal Price { get; set; }

    /// <summary>What the store paid for it. Zero means not recorded yet.</summary>
    public decimal CostPrice { get; set; }

    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Optimistic concurrency token. Stops two sales decrementing the same stock.</summary>
    /// <remarks>Becomes xmin when the provider changes to PostgreSQL.</remarks>
    public byte[]? RowVersion { get; set; }

    /// <summary>Peso margin per unit. Negative on a loss leader, which is allowed.</summary>
    public decimal UnitMargin => Price - CostPrice;
}
