using FluentAssertions;
using TalaanNiAna.Application.Common.Exceptions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class ZReportTests
{
    private readonly FakeTransactionRepository _transactions = new();
    private readonly FakeCustomerRepository _customers = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly ZReportService _service;

    public ZReportTests()
    {
        _service = new ZReportService(_transactions, _customers, _stores);
    }

    [Fact]
    public async Task Expected_drawer_is_cash_sales_plus_collections()
    {
        // Credit sales take no money, so they must stay out of the drawer total.
        _transactions.DaySummary = Day(cash: 1_200m, credit: 800m);
        _customers.PaymentsTotal = 300m;

        var report = await _service.GetAsync();

        report.Cash.ExpectedInDrawer.Should().Be(1_500m);
        report.Sales.GrossSales.Should().Be(2_000m);
    }

    [Fact]
    public async Task Reports_the_day_margin()
    {
        _transactions.DaySummary = Day(cash: 1_000m, revenue: 1_000m, cost: 750m);

        var report = await _service.GetAsync();

        report.Sales.GrossProfit.Should().Be(250m);
        report.Sales.MarginPercent.Should().Be(25m);
    }

    [Fact]
    public async Task Net_credit_change_offsets_new_utang_against_payments()
    {
        _transactions.DaySummary = Day(credit: 500m);
        _customers.PaymentsTotal = 650m;
        _customers.OutstandingCredit = 4_000m;

        var report = await _service.GetAsync();

        report.Credit.NetCreditChange.Should().Be(-150m);
        report.Credit.OutstandingTotal.Should().Be(4_000m);
    }

    [Fact]
    public async Task A_day_with_no_sales_reports_zeros()
    {
        var report = await _service.GetAsync();

        report.Sales.GrossSales.Should().Be(0m);
        report.Sales.AverageSale.Should().Be(0m);
        report.Sales.MarginPercent.Should().Be(0m);
        report.TopSellers.Should().BeEmpty();
    }

    [Fact]
    public async Task Rejects_a_date_that_has_not_happened_yet()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        var act = () => _service.GetAsync(tomorrow);

        await act.Should().ThrowAsync<ConflictException>()
            .Where(e => e.Code == "future_date");
    }

    private static DaySalesSummary Day(
        decimal cash = 0m,
        decimal credit = 0m,
        decimal revenue = 0m,
        decimal cost = 0m,
        int count = 1,
        int items = 1)
        => new(count, items, cash, credit, revenue, cost);
}
