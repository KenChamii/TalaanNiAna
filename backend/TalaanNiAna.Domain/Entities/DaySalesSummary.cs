/// <summary>
/// One day's trading totals, read straight from the database.
/// Cash and credit come from the transactions, revenue and cost from the
/// lines, so the Z-report never has to add anything up in memory.
/// </summary>
public readonly record struct DaySalesSummary(
    int TransactionCount,
    int ItemsSold,
    decimal CashSales,
    decimal CreditSales,
    decimal Revenue,
    decimal Cost);
