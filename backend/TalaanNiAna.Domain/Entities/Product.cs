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

    /// <summary>
    /// The sack, box or pack this is broken out of. A sachet of shampoo points
    /// at the twelve-pack it came from; the pack itself points at nothing.
    /// </summary>
    public int? ParentProductId { get; set; }
    public Product? ParentProduct { get; set; }

    /// <summary>
    /// How many of this come out of one parent. Twelve sachets to a pack, forty
    /// cups to a sack of rice. Null unless this is a tingi line.
    /// </summary>
    public int? UnitsPerParent { get; set; }

    /// <summary>True when opening a parent produces this.</summary>
    public bool IsTingi => ParentProductId is not null && UnitsPerParent > 0;

    /// <summary>Peso margin per unit. Negative on a loss leader, which is allowed.</summary>
    public decimal UnitMargin => Price - CostPrice;
}
