public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize);
    Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<(decimal cashTotal, decimal creditTotal)> GetTodayTotalsAsync();
    Task<IEnumerable<DailySalesPoint>> GetMonthlySalesHistoryAsync();
}