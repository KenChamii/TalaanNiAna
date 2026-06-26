public record DashboardDto(
    decimal TodaySales,
    decimal TotalCollections,
    decimal TotalCredit,
    int LowStockCount,
    IEnumerable<DailySalesPoint> SalesHistory);