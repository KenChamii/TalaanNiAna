public class Transaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentType { get; set; }   // Cash | Credit
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();

    /// <summary>Client-generated key. Unique, so a replayed checkout cannot sell twice.</summary>
    public string? IdempotencyKey { get; set; }
}

public enum PaymentType { Cash, Credit }
