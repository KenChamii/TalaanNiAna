public class Product : IHasStore
{
    public int Id { get; set; }
    public int StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Optimistic concurrency token. Stops two sales decrementing the same stock.</summary>
    /// <remarks>Becomes xmin when the provider changes to PostgreSQL.</remarks>
    public byte[]? RowVersion { get; set; }
}
