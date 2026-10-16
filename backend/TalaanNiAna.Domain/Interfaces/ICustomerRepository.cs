public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? search);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByIdWithTransactionsAsync(int id);
    Task<Customer> CreateAsync(Customer customer);

    /// <summary>
    /// Recomputes TotalCredit from credit transactions minus payments.
    /// Derived rather than incremented, so the cached column cannot drift.
    /// </summary>
    Task<decimal> RecalculateCreditAsync(int customerId);

    Task<int> GetCreditorCountAsync();
    Task<decimal> GetTotalOutstandingCreditAsync();
    Task<CreditPayment> RecordPaymentAsync(int customerId, decimal amount, string? note);
    Task<decimal> GetPaymentsTotalAsync(DateRange range);
    Task<IEnumerable<Customer>> GetTopDebtorsAsync(int count);
}
