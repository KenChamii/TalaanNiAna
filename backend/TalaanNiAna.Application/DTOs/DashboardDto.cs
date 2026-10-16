public record DashboardDto(
    decimal TodaySales,
    decimal TodayProfit,
    decimal TotalCollections,
    decimal TotalCredit,
    int LowStockCount,
    int CreditorCount,
    decimal AverageTransactionValue,
    int MonthlyTransactionCount,
    decimal MonthRevenue,
    decimal MonthProfit,
    decimal MonthMarginPercent,
    IEnumerable<DailySalesPoint> SalesHistory,
    IEnumerable<TopProductResult> TopProducts,
    IEnumerable<TopProductResult> TopProfitable,
    IEnumerable<TopDebtorDto> TopDebtors);

public record TopDebtorDto(int CustomerId, string FullName, decimal TotalCredit);
