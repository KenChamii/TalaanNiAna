public record DailySalesPoint(DateTime Date, decimal CashSales, decimal CreditSales)
{
    public decimal TotalSales => CashSales + CreditSales;
}