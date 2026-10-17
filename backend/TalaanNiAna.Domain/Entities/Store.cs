/// <summary>
/// The tenant. One row per sari-sari store, created at registration, and
/// also where that store's settings live.
/// </summary>
public class Store
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>IANA id. Decides where the sales day starts and ends.</summary>
    public string TimeZoneId { get; set; } = "Asia/Manila";

    /// <summary>Default threshold for new products. Each product can override it.</summary>
    public int LowStockAlertLimit { get; set; } = 10;

    public string SmsAlertTemplate { get; set; } = "Hi Suki, paalala lang...";

    /// <summary>
    /// The public demo store. Marked rather than matched by name, so the seeder
    /// can tell whether it has already run without caring what the store is called.
    /// </summary>
    public bool IsDemo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
