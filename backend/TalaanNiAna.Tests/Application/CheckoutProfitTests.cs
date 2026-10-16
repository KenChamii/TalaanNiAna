using FluentAssertions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class CheckoutProfitTests
{
    private readonly FakeUnitOfWork _uow = new();
    private readonly CheckoutService _service;

    public CheckoutProfitTests()
    {
        _service = new CheckoutService(_uow);
    }

    [Fact]
    public async Task Snapshots_the_cost_at_the_moment_of_sale()
    {
        var product = _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, cost: 22m);

        await _service.ProcessSaleAsync(Sale(quantity: 2));

        // Supplier raises the price after the sale.
        product.CostPrice = 28m;

        var line = _uow.TransactionsFake.Saved.Single().Items.Single();
        line.UnitCost.Should().Be(22m);
        line.Profit.Should().Be(16m);   // 2 x (30 - 22)
    }

    [Fact]
    public async Task Records_zero_cost_when_none_has_been_entered()
    {
        _uow.ProductsFake.Add(id: 1, name: "Bagong item", price: 30m, stock: 10);

        await _service.ProcessSaleAsync(Sale(quantity: 1));

        _uow.TransactionsFake.Saved.Single().Items.Single().UnitCost.Should().Be(0m);
    }

    [Fact]
    public async Task Allows_a_loss_leader()
    {
        // Sold below cost on purpose, to pull people into the store.
        _uow.ProductsFake.Add(id: 1, name: "Asin", price: 5m, stock: 10, cost: 7m);

        await _service.ProcessSaleAsync(Sale(quantity: 1, unitPrice: 5m));

        _uow.TransactionsFake.Saved.Single().Items.Single().Profit.Should().Be(-2m);
    }

    private static CheckoutDto Sale(int quantity, decimal unitPrice = 30m)
        => new("Cash", null, [new CheckoutItemDto(1, quantity, unitPrice)], "test-key-0002");
}
