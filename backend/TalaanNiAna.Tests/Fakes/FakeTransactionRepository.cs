namespace TalaanNiAna.Tests.Fakes;

public sealed class FakeTransactionRepository : ITransactionRepository
{
    private int _nextId = 1;

    public List<Transaction> Saved { get; } = [];

    /// <summary>Returned by GetDaySummaryAsync. Set by the report tests.</summary>
    public DaySalesSummary DaySummary { get; set; }

    /// <summary>Returned by GetTopProductsAsync. Set by the report tests.</summary>
    public List<TopProductResult> TopProducts { get; } = [];

    public Task<Transaction> CreateAsync(Transaction transaction)
    {
        transaction.Id = _nextId++;
        Saved.Add(transaction);
        return Task.FromResult(transaction);
    }

    public Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        => Task.FromResult(Saved.FirstOrDefault(t => t.IdempotencyKey == idempotencyKey));

    public Task<DaySalesSummary> GetDaySummaryAsync(DateRange range)
        => Task.FromResult(DaySummary);

    public Task<IEnumerable<TopProductResult>> GetTopProductsAsync(DateRange range, int count)
        => Task.FromResult(TopProducts.Take(count));

    // Not exercised by these tests.
    public Task<IEnumerable<Transaction>> GetAllAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page, int pageSize)
        => throw new NotSupportedException();
    public Task<int> GetTotalCountAsync(DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType)
        => throw new NotSupportedException();
    public Task<(decimal cashTotal, decimal creditTotal)> GetTotalsAsync(DateRange range)
        => throw new NotSupportedException();
    public Task<(decimal revenue, decimal cost)> GetMarginAsync(DateRange range)
        => throw new NotSupportedException();
    public Task<IEnumerable<DailySalesPoint>> GetDailySalesAsync(DateRange range, int utcOffsetHours)
        => throw new NotSupportedException();
    public Task<(decimal averageValue, int count)> GetAverageTransactionAsync(DateRange range)
        => throw new NotSupportedException();
    public Task<IEnumerable<TopProductResult>> GetTopProfitableAsync(DateRange range, int count)
        => throw new NotSupportedException();
}
