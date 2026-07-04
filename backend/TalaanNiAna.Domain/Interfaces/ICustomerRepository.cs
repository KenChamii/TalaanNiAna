public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? search);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByIdWithTransactionsAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task UpdateCreditAsync(int customerId, decimal delta); // positive = adds debt, negative = payment
    Task<int> GetCreditorCountAsync(); // customers with TotalCredit > 0
    Task<CreditPayment> RecordPaymentAsync(int customerId, decimal amount, string? note);
    Task<decimal> GetTodayPaymentsTotalAsync();
    Task<IEnumerable<Customer>> GetTopDebtorsAsync(int count);
}