public class DashboardService
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IProductRepository _productRepo;
    private readonly StoreSettings _defaultSettings; // see 4.10 — used for the low-stock threshold

    public DashboardService(
        ITransactionRepository transactionRepo,
        ICustomerRepository customerRepo,
        IProductRepository productRepo,
        IStoreSettingsRepository settingsRepo)
    {
        _transactionRepo = transactionRepo;
        _customerRepo = customerRepo;
        _productRepo = productRepo;
        _defaultSettings = settingsRepo.GetAsync().GetAwaiter().GetResult(); // safe one-time read at startup of each scoped request
    }

    public async Task<DashboardDto> GetSummaryAsync()
    {
        var (cashToday, creditToday) = await _transactionRepo.GetTodayTotalsAsync();
        var totalCreditOutstanding = await GetTotalOutstandingCreditAsync();
        var lowStockItems = await _productRepo.GetLowStockAsync(_defaultSettings.LowStockAlertLimit);
        var history = await _transactionRepo.GetMonthlySalesHistoryAsync();

        var creditorCount = await _customerRepo.GetCreditorCountAsync();
        var todayPaymentsCollected = await _customerRepo.GetTodayPaymentsTotalAsync();
        var (avgTransactionValue, monthlyCount) = await _transactionRepo.GetMonthlyAverageTransactionAsync();
        var topProducts = await _transactionRepo.GetTopProductsThisMonthAsync(5);
        var topDebtors = await _customerRepo.GetTopDebtorsAsync(5);

        return new DashboardDto(
            TodaySales: cashToday + creditToday,
            TotalCollections: todayPaymentsCollected, // actual utang payments collected today, not cash sales
            TotalCredit: totalCreditOutstanding,
            LowStockCount: lowStockItems.Count(),
            CreditorCount: creditorCount,
            AverageTransactionValue: avgTransactionValue,
            MonthlyTransactionCount: monthlyCount,
            SalesHistory: history,
            TopProducts: topProducts,
            TopDebtors: topDebtors.Select(c => new TopDebtorDto(c.Id, c.FullName, c.TotalCredit)));
    }

    private async Task<decimal> GetTotalOutstandingCreditAsync()
    {
        var customers = await _customerRepo.GetAllAsync(search: null, page: 1, pageSize: int.MaxValue);
        return customers.Sum(c => c.TotalCredit);
    }
}
