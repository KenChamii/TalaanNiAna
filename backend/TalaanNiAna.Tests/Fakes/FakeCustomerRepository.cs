namespace TalaanNiAna.Tests.Fakes;

public sealed class FakeCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<int, Customer> _customers = [];

    /// <summary>Customer ids passed to RecalculateCreditAsync, in order.</summary>
    public List<int> Recalculated { get; } = [];

    /// <summary>Utang payments for the window. Set by the report tests.</summary>
    public decimal PaymentsTotal { get; set; }

    /// <summary>Utang still owed overall. Set by the report tests.</summary>
    public decimal OutstandingCredit { get; set; }

    public Customer Add(int id, string name, decimal credit = 0)
    {
        var customer = new Customer { Id = id, FullName = name, TotalCredit = credit };
        _customers[id] = customer;
        return customer;
    }

    public Task<Customer?> GetByIdAsync(int id)
        => Task.FromResult(_customers.GetValueOrDefault(id));

    public Task<decimal> RecalculateCreditAsync(int customerId)
    {
        Recalculated.Add(customerId);
        return Task.FromResult(_customers.TryGetValue(customerId, out var c) ? c.TotalCredit : 0m);
    }

    public Task<decimal> GetPaymentsTotalAsync(DateRange range) => Task.FromResult(PaymentsTotal);

    public Task<decimal> GetTotalOutstandingCreditAsync() => Task.FromResult(OutstandingCredit);

    // Not exercised by these tests.
    public Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize)
        => throw new NotSupportedException();
    public Task<int> GetTotalCountAsync(string? search) => throw new NotSupportedException();
    public Task<Customer?> GetByIdWithTransactionsAsync(int id) => throw new NotSupportedException();
    public Task<Customer> CreateAsync(Customer customer) => throw new NotSupportedException();
    public Task<int> GetCreditorCountAsync() => throw new NotSupportedException();
    public Task<CreditPayment> RecordPaymentAsync(int customerId, decimal amount, string? note)
        => throw new NotSupportedException();
    public Task<IEnumerable<Customer>> GetTopDebtorsAsync(int count) => throw new NotSupportedException();
}
