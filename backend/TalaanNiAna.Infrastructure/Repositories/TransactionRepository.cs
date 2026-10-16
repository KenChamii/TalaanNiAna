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
        => await Filter(dateFrom, dateTo, paymentType)
            .Include(t => t.Customer)
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType)
        => await Filter(dateFrom, dateTo, paymentType).CountAsync();

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        => await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);

    public async Task<(decimal cashTotal, decimal creditTotal)> GetTotalsAsync(DateRange range)
    {
        // One round trip, both totals summed by the database.
        var totals = await InRange(range)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Cash = g.Sum(t => t.PaymentType == PaymentType.Cash ? t.TotalAmount : 0m),
                Credit = g.Sum(t => t.PaymentType == PaymentType.Credit ? t.TotalAmount : 0m)
            })
            .FirstOrDefaultAsync();

        return (totals?.Cash ?? 0m, totals?.Credit ?? 0m);
    }

    public async Task<(decimal revenue, decimal cost)> GetMarginAsync(DateRange range)
    {
        var margin = await ItemsInRange(range)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice),
                Cost = g.Sum(i => i.Quantity * i.UnitCost)
            })
            .FirstOrDefaultAsync();

        return (margin?.Revenue ?? 0m, margin?.Cost ?? 0m);
    }

    public async Task<IEnumerable<DailySalesPoint>> GetDailySalesAsync(DateRange range, int utcOffsetHours)
    {
        // Shift to local time before taking the date, so a sale at 10pm Manila
        // lands on the day it was made rather than the next UTC day.
        var grouped = await InRange(range)
            .GroupBy(t => t.TransactionDate.AddHours(utcOffsetHours).Date)
            .Select(g => new
            {
                Date = g.Key,
                CashSales = g.Sum(t => t.PaymentType == PaymentType.Cash ? t.TotalAmount : 0m),
                CreditSales = g.Sum(t => t.PaymentType == PaymentType.Credit ? t.TotalAmount : 0m)
            })
            .OrderBy(g => g.Date)
            .ToListAsync();

        return grouped.Select(g => new DailySalesPoint(g.Date, g.CashSales, g.CreditSales));
    }

    public async Task<(decimal averageValue, int count)> GetAverageTransactionAsync(DateRange range)
    {
        var summary = await InRange(range)
            .GroupBy(_ => 1)
            .Select(g => new { Count = g.Count(), Total = g.Sum(t => t.TotalAmount) })
            .FirstOrDefaultAsync();

        if (summary is null || summary.Count == 0) return (0m, 0);

        return (summary.Total / summary.Count, summary.Count);
    }

    public async Task<IEnumerable<TopProductResult>> GetTopProductsAsync(DateRange range, int count)
        => await TopBy(range, count, byProfit: false);

    public async Task<IEnumerable<TopProductResult>> GetTopProfitableAsync(DateRange range, int count)
        => await TopBy(range, count, byProfit: true);

    private async Task<IEnumerable<TopProductResult>> TopBy(DateRange range, int count, bool byProfit)
    {
        // Project to a flat, navigation-free shape first so the GroupBy key and
        // aggregates are plain scalars — mixing navigation properties into a
        // GroupBy key alongside aggregates can fail to translate to SQL.
        var flatItems = ItemsInRange(range)
            .Select(i => new
            {
                i.ProductId,
                ProductName = i.Product.Name,
                i.Quantity,
                i.UnitPrice,
                i.UnitCost
            });

        var grouped = flatItems
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                ProductName = g.Select(i => i.ProductName).FirstOrDefault(),
                TotalQuantitySold = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice),
                TotalProfit = g.Sum(i => i.Quantity * (i.UnitPrice - i.UnitCost))
            });

        grouped = byProfit
            ? grouped.OrderByDescending(r => r.TotalProfit)
            : grouped.OrderByDescending(r => r.TotalQuantitySold);

        var rows = await grouped.Take(count).ToListAsync();

        return rows.Select(g => new TopProductResult(
            g.ProductId,
            g.ProductName ?? "(walang pangalan)",
            g.TotalQuantitySold,
            g.TotalRevenue,
            g.TotalProfit));
    }

    private IQueryable<Transaction> InRange(DateRange range)
        => _context.Transactions
            .Where(t => t.TransactionDate >= range.FromUtc && t.TransactionDate < range.ToUtc);

    private IQueryable<TransactionItem> ItemsInRange(DateRange range)
        => _context.TransactionItems
            .Where(i => i.Transaction.TransactionDate >= range.FromUtc
                     && i.Transaction.TransactionDate < range.ToUtc);

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
