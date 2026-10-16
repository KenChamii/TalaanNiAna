public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize);
    Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType);
    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>Returns the original transaction for a replayed checkout, if any.</summary>
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

    Task<(decimal cashTotal, decimal creditTotal)> GetTotalsAsync(DateRange range);

    /// <summary>Daily totals grouped by the store's local date, not UTC.</summary>
    Task<IEnumerable<DailySalesPoint>> GetDailySalesAsync(DateRange range, int utcOffsetHours);

    Task<(decimal averageValue, int count)> GetAverageTransactionAsync(DateRange range);
    Task<IEnumerable<TopProductResult>> GetTopProductsAsync(DateRange range, int count);
}
