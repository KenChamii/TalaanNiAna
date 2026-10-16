using TalaanNiAna.Application.Common;
using TalaanNiAna.Application.Common.Exceptions;

/// <summary>
/// Builds the end-of-day close for one store-local date. Read-only, so the
/// same day can be reprinted as often as the owner needs.
/// </summary>
public class ZReportService
{
    private const int TopSellerCount = 5;

    private readonly ITransactionRepository _transactions;
    private readonly ICustomerRepository _customers;
    private readonly IStoreRepository _stores;

    public ZReportService(
        ITransactionRepository transactions,
        ICustomerRepository customers,
        IStoreRepository stores)
    {
        _transactions = transactions;
        _customers = customers;
        _stores = stores;
    }

    /// <param name="businessDate">Store-local date. Defaults to today.</param>
    public async Task<ZReportDto> GetAsync(DateOnly? businessDate = null)
    {
        var store = await _stores.GetCurrentAsync();
        var clock = new StoreClock(store.TimeZoneId);

        var date = businessDate ?? clock.LocalToday;

        if (date > clock.LocalToday)
            throw new ConflictException("Hindi pa dumarating ang petsang iyon.", "future_date");

        var range = clock.Day(date);

        var summary = await _transactions.GetDaySummaryAsync(range);
        var topSellers = await _transactions.GetTopProductsAsync(range, TopSellerCount);
        var collections = await _customers.GetPaymentsTotalAsync(range);

        // As of now, not as of the chosen date: the owner wants to know what is
        // still owed today, even when reprinting an older close.
        var outstanding = await _customers.GetTotalOutstandingCreditAsync();

        var grossSales = summary.CashSales + summary.CreditSales;
        var grossProfit = summary.Revenue - summary.Cost;

        return new ZReportDto(
            StoreName: store.Name,
            BusinessDate: date,
            GeneratedAtUtc: DateTime.UtcNow,
            Sales: new ZReportSalesDto(
                TransactionCount: summary.TransactionCount,
                ItemsSold: summary.ItemsSold,
                GrossSales: grossSales,
                AverageSale: Average(grossSales, summary.TransactionCount),
                CostOfGoods: summary.Cost,
                GrossProfit: grossProfit,
                MarginPercent: Percent(grossProfit, summary.Revenue)),
            Cash: new ZReportCashDto(
                CashSales: summary.CashSales,
                CollectionsReceived: collections,
                ExpectedInDrawer: summary.CashSales + collections),
            Credit: new ZReportCreditDto(
                NewCreditIssued: summary.CreditSales,
                CollectionsReceived: collections,
                NetCreditChange: summary.CreditSales - collections,
                OutstandingTotal: outstanding),
            TopSellers: topSellers.Select(p => new ZReportLineDto(
                p.ProductName, p.TotalQuantitySold, p.TotalRevenue, p.TotalProfit)));
    }

    private static decimal Average(decimal total, int count)
        => count == 0 ? 0 : Math.Round(total / count, 2);

    private static decimal Percent(decimal part, decimal whole)
        => whole <= 0 ? 0 : Math.Round(part / whole * 100, 1);
}
