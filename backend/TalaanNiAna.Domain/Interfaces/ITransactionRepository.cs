public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize);
    Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType);
    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>Returns the original transaction for a replayed checkout, if any.</summary>
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

    Task<(decimal cashTotal, decimal creditTotal)> GetTotalsAsync(DateRange range);

    /// <summary>Revenue and cost of goods sold, from the snapshotted line costs.</summary>
    Task<(decimal revenue, decimal cost)> GetMarginAsync(DateRange range);

    /// <summary>Daily totals grouped by the store's local date, not UTC.</summary>
    Task<IEnumerable<DailySalesPoint>> GetDailySalesAsync(DateRange range, int utcOffsetHours);

    Task<(decimal averageValue, int count)> GetAverageTransactionAsync(DateRange range);

    /// <summary>Best sellers by units moved.</summary>
    Task<IEnumerable<TopProductResult>> GetTopProductsAsync(DateRange range, int count);

    /// <summary>Best earners by peso profit. Rarely the same list.</summary>
    Task<IEnumerable<TopProductResult>> GetTopProfitableAsync(DateRange range, int count);
}
