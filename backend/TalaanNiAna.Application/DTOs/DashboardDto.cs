public record DashboardDto(
    decimal TodaySales,
    decimal TotalCollections,
    decimal TotalCredit,
    int LowStockCount,
    int CreditorCount,
    decimal AverageTransactionValue,
    int MonthlyTransactionCount,
    IEnumerable<DailySalesPoint> SalesHistory,
    IEnumerable<TopProductResult> TopProducts,
    IEnumerable<TopDebtorDto> TopDebtors);

public record TopDebtorDto(int CustomerId, string FullName, decimal TotalCredit);
