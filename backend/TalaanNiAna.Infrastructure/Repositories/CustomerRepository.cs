using Microsoft.EntityFrameworkCore;
using TalaanNiAna.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize)
        => await Filter(search)
            .OrderByDescending(c => c.LastActiveAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync(string? search)
        => await Filter(search).CountAsync();

    public async Task<Customer?> GetByIdAsync(int id)
        => await _context.Customers.FindAsync(id);

    public async Task<Customer?> GetByIdWithTransactionsAsync(int id)
        => await _context.Customers
            .Include(c => c.Transactions.OrderByDescending(t => t.TransactionDate))
                .ThenInclude(t => t.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<decimal> RecalculateCreditAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId)
            ?? throw new InvalidOperationException($"Customer {customerId} not found.");

        var charged = await _context.Transactions
            .Where(t => t.CustomerId == customerId && t.PaymentType == PaymentType.Credit)
            .SumAsync(t => (decimal?)t.TotalAmount) ?? 0m;

        var paid = await _context.CreditPayments
            .Where(p => p.CustomerId == customerId)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        customer.TotalCredit = charged - paid;
        customer.LastActiveAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return customer.TotalCredit;
    }

    public async Task<int> GetCreditorCountAsync()
        => await _context.Customers.CountAsync(c => c.TotalCredit > 0);

    public async Task<decimal> GetTotalOutstandingCreditAsync()
        => await _context.Customers
            .Where(c => c.TotalCredit > 0)
            .SumAsync(c => (decimal?)c.TotalCredit) ?? 0m;

    public async Task<CreditPayment> RecordPaymentAsync(int customerId, decimal amount, string? note)
    {
        _ = await _context.Customers.FindAsync(customerId)
            ?? throw new InvalidOperationException($"Customer {customerId} not found.");

        var payment = new CreditPayment
        {
            CustomerId = customerId,
            Amount = amount,
            PaidAt = DateTime.UtcNow,
            Note = note
        };

        _context.CreditPayments.Add(payment);
        await _context.SaveChangesAsync();

        // The caller recalculates, so the balance is derived in one pass.
        return payment;
    }

    public async Task<decimal> GetPaymentsTotalAsync(DateRange range)
        => await _context.CreditPayments
            .Where(p => p.PaidAt >= range.FromUtc && p.PaidAt < range.ToUtc)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

    public async Task<IEnumerable<Customer>> GetTopDebtorsAsync(int count)
        => await _context.Customers
            .AsNoTracking()
            .Where(c => c.TotalCredit > 0)
            .OrderByDescending(c => c.TotalCredit)
            .Take(count)
            .ToListAsync();

    private IQueryable<Customer> Filter(string? search)
    {
        var query = _context.Customers.AsQueryable();

        // ILIKE, because Postgres compares case-sensitively and a cashier
        // typing "ana" should still find Ana.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = SearchPattern.Contains(search);
            query = query.Where(c => EF.Functions.ILike(c.FullName, pattern, SearchPattern.Escape));
        }

        return query;
    }
}
