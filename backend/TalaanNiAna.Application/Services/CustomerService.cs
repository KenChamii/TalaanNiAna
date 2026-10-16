using TalaanNiAna.Application.Common.Exceptions;

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

    public async Task<CreditPaymentDto> RecordPaymentAsync(int customerId, RecordPaymentDto dto)
    {
        var customer = await _repo.GetByIdAsync(customerId)
            ?? throw new NotFoundException("Suki", customerId);

        if (dto.Amount > customer.TotalCredit)
            throw new ConflictException(
                $"Mas malaki ang bayad ({dto.Amount:N2}) kaysa sa utang ({customer.TotalCredit:N2}).",
                "overpayment");

        var payment = await _repo.RecordPaymentAsync(customerId, dto.Amount, dto.Note);
        var remaining = await _repo.RecalculateCreditAsync(customerId);

        return new CreditPaymentDto(
            payment.Id, payment.Amount, payment.PaidAt, payment.Note, remaining);
    }

    private static CustomerDto ToDto(Customer c) => new(
        c.Id, c.FullName, c.ContactNumber, c.TotalCredit, c.LastActiveAt);
}
