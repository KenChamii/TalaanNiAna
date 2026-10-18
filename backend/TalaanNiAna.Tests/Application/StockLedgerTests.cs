using FluentAssertions;
using TalaanNiAna.Application.Common.Exceptions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class StockLedgerTests
{
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StockService _service;

    public StockLedgerTests()
    {
        _service = new StockService(_uow, _stores);
    }

    [Fact]
    public async Task A_delivery_raises_stock_and_records_why()
    {
        var product = _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var movement = await _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: 24, Reason: "Restock", Note: "Delivery"));

        product.StockQuantity.Should().Be(34);
        movement.BalanceAfter.Should().Be(34);
        movement.Reason.Should().Be("Restock");
        movement.Note.Should().Be("Delivery");
    }

    [Fact]
    public async Task Spoilage_lowers_stock()
    {
        var product = _uow.ProductsFake.Add(id: 1, name: "Itlog", price: 9m, stock: 30);

        await _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: -6, Reason: "Spoilage", Note: "Nabasag"));

        product.StockQuantity.Should().Be(24);
    }

    [Fact]
    public async Task Spoilage_that_adds_stock_is_a_typo()
    {
        _uow.ProductsFake.Add(id: 1, name: "Itlog", price: 9m, stock: 30);

        var act = () => _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: 6, Reason: "Spoilage"));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task Stock_cannot_be_taken_below_zero()
    {
        _uow.ProductsFake.Add(id: 1, name: "Asin", price: 5m, stock: 3);

        var act = () => _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: -5, Reason: "Spoilage"));

        await act.Should().ThrowAsync<ConflictException>()
            .Where(e => e.Code == "insufficient_stock");
    }

    [Fact]
    public async Task A_sale_is_not_something_the_owner_records_by_hand()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var act = () => _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: -1, Reason: "Sale"));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task Selling_writes_a_movement_against_the_sale()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, cost: 22m);
        var checkout = new CheckoutService(_uow);

        await checkout.ProcessSaleAsync(new CheckoutDto(
            "Cash", null, [new CheckoutItemDto(1, 3, 30m)], "ledger-key-0001"));

        var movement = _uow.StockMovementsFake.Movements.Single();
        movement.Quantity.Should().Be(-3);
        movement.Reason.Should().Be(StockReason.Sale);
        movement.BalanceAfter.Should().Be(7);
        movement.TransactionId.Should().NotBeNull();
    }

    [Fact]
    public async Task Shrinkage_values_the_loss_at_what_it_cost()
    {
        _uow.ProductsFake.Add(id: 1, name: "Itlog", price: 9m, stock: 30, cost: 7m);

        await _service.RecordAsync(
            new RecordStockDto(ProductId: 1, Quantity: -6, Reason: "Spoilage", Note: "Nabasag"));

        var report = await _service.GetShrinkageAsync();

        report.TotalPieces.Should().Be(6);
        report.TotalCost.Should().Be(42m);   // 6 x 7
    }
}
