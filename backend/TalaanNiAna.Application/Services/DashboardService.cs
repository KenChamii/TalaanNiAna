using TalaanNiAna.Application.Common;

public class DashboardService
{
    private const int TopListSize = 5;

    private readonly ITransactionRepository _transactionRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IProductRepository _productRepo;
    private readonly IStoreRepository _stores;

    public DashboardService(
        ITransactionRepository transactionRepo,
        ICustomerRepository customerRepo,
        IProductRepository productRepo,
        IStoreRepository stores)
    {
        _transactionRepo = transactionRepo;
        _customerRepo = customerRepo;
        _productRepo = productRepo;
        _stores = stores;
    }

    public async Task<DashboardDto> GetSummaryAsync()
    {
        var store = await _stores.GetCurrentAsync();
        var clock = new StoreClock(store.TimeZoneId);

        var today = clock.Today();
        var month = clock.MonthToDate();

        var (cashToday, creditToday) = await _transactionRepo.GetTotalsAsync(today);
        var (todayRevenue, todayCost) = await _transactionRepo.GetMarginAsync(today);
        var (monthRevenue, monthCost) = await _transactionRepo.GetMarginAsync(month);

        var paymentsToday = await _customerRepo.GetPaymentsTotalAsync(today);
        var totalCredit = await _customerRepo.GetTotalOutstandingCreditAsync();
        var creditorCount = await _customerRepo.GetCreditorCountAsync();
        var lowStockCount = await _productRepo.GetLowStockCountAsync();

        var history = await _transactionRepo.GetDailySalesAsync(month, clock.UtcOffsetHours);
        var (averageValue, monthlyCount) = await _transactionRepo.GetAverageTransactionAsync(month);
        var topProducts = await _transactionRepo.GetTopProductsAsync(month, TopListSize);
        var topProfitable = await _transactionRepo.GetTopProfitableAsync(month, TopListSize);
        var topDebtors = await _customerRepo.GetTopDebtorsAsync(TopListSize);

        var monthProfit = monthRevenue - monthCost;

        return new DashboardDto(
            TodaySales: cashToday + creditToday,
            TodayProfit: todayRevenue - todayCost,
            TotalCollections: paymentsToday,   // utang payments collected, not cash sales
            TotalCredit: totalCredit,
            LowStockCount: lowStockCount,
            CreditorCount: creditorCount,
            AverageTransactionValue: averageValue,
            MonthlyTransactionCount: monthlyCount,
            MonthRevenue: monthRevenue,
            MonthProfit: monthProfit,
            MonthMarginPercent: Percent(monthProfit, monthRevenue),
            SalesHistory: history,
            TopProducts: topProducts,
            TopProfitable: topProfitable,
            TopDebtors: topDebtors.Select(c => new TopDebtorDto(c.Id, c.FullName, c.TotalCredit)));
    }

    private static decimal Percent(decimal part, decimal whole)
        => whole <= 0 ? 0 : Math.Round(part / whole * 100, 1);
}
