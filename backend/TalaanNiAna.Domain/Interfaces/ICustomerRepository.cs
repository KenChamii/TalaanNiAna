public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? search);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByIdWithTransactionsAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task UpdateCreditAsync(int customerId, decimal delta); // positive = adds debt, negative = payment
}