public class Customer : IHasStore
{
    public int Id { get; set; }
    public int StoreId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }

    /// <summary>Cached balance. Derived from the ledger by RecalculateCreditAsync.</summary>
    public decimal TotalCredit { get; set; } = 0;

    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<CreditPayment> CreditPayments { get; set; } = new List<CreditPayment>();
}
