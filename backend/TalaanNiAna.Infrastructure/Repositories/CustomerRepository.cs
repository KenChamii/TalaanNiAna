using Microsoft.EntityFrameworkCore;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(string? search, int page, int pageSize)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.FullName.Contains(search));

        return await query
            .OrderByDescending(c => c.LastActiveAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search)
    {
        var query = _context.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.FullName.Contains(search));
        return await query.CountAsync();
    }

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

    public async Task UpdateCreditAsync(int customerId, decimal delta)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer is null) return;

        customer.TotalCredit += delta;
        customer.LastActiveAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}