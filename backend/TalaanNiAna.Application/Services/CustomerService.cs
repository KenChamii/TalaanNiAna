public class CustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<CustomerDto>> GetCustomersAsync(
        string? search, int page = 1, int pageSize = 10)
    {
        var customers = await _repo.GetAllAsync(search, page, pageSize);
        var total = await _repo.GetTotalCountAsync(search);

        return new PagedResult<CustomerDto>
        {
            Items = customers.Select(ToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<CustomerDetailDto?> GetDetailAsync(int id)
    {
        var customer = await _repo.GetByIdWithTransactionsAsync(id);
        if (customer is null) return null;

        return new CustomerDetailDto(
            customer.Id, customer.FullName, customer.ContactNumber,
            customer.TotalCredit, customer.LastActiveAt,
            customer.Transactions.Select(t => new CustomerTransactionDto(
                t.Id, t.TransactionDate, t.TotalAmount, t.PaymentType.ToString(),
                t.Items.Select(i => $"{i.Quantity}x {i.Product.Name}"))));
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FullName = dto.FullName,
            ContactNumber = dto.ContactNumber
        };

        var created = await _repo.CreateAsync(customer);
        return ToDto(created);
    }

    private static CustomerDto ToDto(Customer c) => new(
        c.Id, c.FullName, c.ContactNumber, c.TotalCredit, c.LastActiveAt);
}