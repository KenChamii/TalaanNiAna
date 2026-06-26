using Microsoft.EntityFrameworkCore;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize)
    {
        var query = Filter(dateFrom, dateTo, paymentType);

        return await query
            .Include(t => t.Customer)
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType)
        => await Filter(dateFrom, dateTo, paymentType).CountAsync();

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<(decimal cashTotal, decimal creditTotal)> GetTodayTotalsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var todays = await _context.Transactions
            .Where(t => t.TransactionDate >= today && t.TransactionDate < today.AddDays(1))
            .ToListAsync();

        var cash = todays.Where(t => t.PaymentType == PaymentType.Cash).Sum(t => t.TotalAmount);
        var credit = todays.Where(t => t.PaymentType == PaymentType.Credit).Sum(t => t.TotalAmount);
        return (cash, credit);
    }

    public async Task<IEnumerable<DailySalesPoint>> GetMonthlySalesHistoryAsync()
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var grouped = await _context.Transactions
            .Where(t => t.TransactionDate >= monthStart)
            .GroupBy(t => t.TransactionDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                CashSales = g.Where(t => t.PaymentType == PaymentType.Cash).Sum(t => t.TotalAmount),
                CreditSales = g.Where(t => t.PaymentType == PaymentType.Credit).Sum(t => t.TotalAmount)
            })
            .OrderBy(g => g.Date)
            .ToListAsync();

        return grouped.Select(g => new DailySalesPoint(g.Date, g.CashSales, g.CreditSales));
    }

    private IQueryable<Transaction> Filter(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType)
    {
        var query = _context.Transactions.AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(t => t.TransactionDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(t => t.TransactionDate < dateTo.Value.AddDays(1));

        if (paymentType.HasValue)
            query = query.Where(t => t.PaymentType == paymentType.Value);

        return query;
    }
}