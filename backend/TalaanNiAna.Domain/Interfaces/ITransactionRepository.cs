public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize);

    Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType);

    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>Finds an earlier sale with the same key, so a replay returns it.</summary>
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

    Task<(decimal cashTotal, decimal creditTotal)> GetTotalsAsync(DateRange range);

    /// <summary>Revenue and cost of goods for the window, summed by the database.</summary>
    Task<(decimal revenue, decimal cost)> GetMarginAsync(DateRange range);

    /// <summary>Everything the end-of-day report needs, in two queries.</summary>
    Task<DaySalesSummary> GetDaySummaryAsync(DateRange range);

    Task<IEnumerable<DailySalesPoint>> GetDailySalesAsync(DateRange range, int utcOffsetHours);

    Task<(decimal averageValue, int count)> GetAverageTransactionAsync(DateRange range);

    Task<IEnumerable<TopProductResult>> GetTopProductsAsync(DateRange range, int count);

    Task<IEnumerable<TopProductResult>> GetTopProfitableAsync(DateRange range, int count);
}
