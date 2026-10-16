/// <summary>
/// End-of-day close. Everything the owner needs to count the drawer and see
/// whether the day made money, for one store-local date.
/// </summary>
public record ZReportDto(
    string StoreName,
    DateOnly BusinessDate,
    DateTime GeneratedAtUtc,
    ZReportSalesDto Sales,
    ZReportCashDto Cash,
    ZReportCreditDto Credit,
    IEnumerable<ZReportLineDto> TopSellers);

public record ZReportSalesDto(
    int TransactionCount,
    int ItemsSold,
    decimal GrossSales,
    decimal AverageSale,
    decimal CostOfGoods,
    decimal GrossProfit,
    decimal MarginPercent);

/// <summary>
/// What should be in the drawer at closing: cash sales plus utang payments
/// collected today. Credit sales are excluded, no money changed hands.
/// </summary>
public record ZReportCashDto(
    decimal CashSales,
    decimal CollectionsReceived,
    decimal ExpectedInDrawer);

public record ZReportCreditDto(
    decimal NewCreditIssued,
    decimal CollectionsReceived,
    decimal NetCreditChange,
    decimal OutstandingTotal);

public record ZReportLineDto(
    string ProductName,
    int QuantitySold,
    decimal Revenue,
    decimal Profit);
