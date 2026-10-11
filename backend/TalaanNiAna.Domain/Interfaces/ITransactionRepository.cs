public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize);
    Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType);
    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>Returns the original transaction for a replayed checkout, if any.</summary>
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

    Task<(decimal cashTotal, decimal creditTotal)> GetTodayTotalsAsync();
    Task<IEnumerable<DailySalesPoint>> GetMonthlySalesHistoryAsync();
    Task<(decimal averageValue, int count)> GetMonthlyAverageTransactionAsync();
    Task<IEnumerable<TopProductResult>> GetTopProductsThisMonthAsync(int count);
}
