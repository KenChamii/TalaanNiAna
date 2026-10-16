using FluentAssertions;
using TalaanNiAna.Application.Common.Exceptions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class CheckoutServiceTests
{
    private readonly FakeUnitOfWork _uow = new();
    private readonly CheckoutService _service;

    public CheckoutServiceTests()
    {
        _service = new CheckoutService(_uow);
    }

    [Fact]
    public async Task Charges_the_price_on_the_product_row()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var result = await _service.ProcessSaleAsync(Sale([Item(1, 2, 30m)]));

        result.TotalAmount.Should().Be(60m);
        _uow.TransactionsFake.Saved.Single().Items.Single().UnitPrice.Should().Be(30m);
    }

    [Fact]
    public async Task Rejects_a_price_the_client_made_up()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        // The exploit this guards: one centavo for anything on the shelf.
        var act = () => _service.ProcessSaleAsync(Sale([Item(1, 1, 0.01m)]));

        var error = await act.Should().ThrowAsync<ConflictException>();
        error.Which.Code.Should().Be("price_changed");
        _uow.TransactionsFake.Saved.Should().BeEmpty();
    }

    [Fact]
    public async Task Refuses_to_sell_more_than_is_in_stock()
    {
        _uow.ProductsFake.Add(id: 1, name: "Tide sachet", price: 12m, stock: 2);

        var act = () => _service.ProcessSaleAsync(Sale([Item(1, 5, 12m)]));

        var error = await act.Should().ThrowAsync<ConflictException>();
        error.Which.Code.Should().Be("insufficient_stock");
        error.Which.Message.Should().Contain("Tide sachet").And.Contain("2").And.Contain("5");
    }

    [Fact]
    public async Task Deducts_stock_on_a_completed_sale()
    {
        _uow.ProductsFake.Add(id: 1, name: "Pancit Canton", price: 17m, stock: 10);

        await _service.ProcessSaleAsync(Sale([Item(1, 3, 17m)]));

        _uow.ProductsFake[1].StockQuantity.Should().Be(7);
        _uow.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Rolls_back_when_a_later_line_fails()
    {
        _uow.ProductsFake.Add(id: 1, name: "Sky Flakes", price: 9m, stock: 10);
        _uow.ProductsFake.Add(id: 2, name: "Piattos", price: 20m, stock: 1);

        var act = () => _service.ProcessSaleAsync(Sale([Item(1, 1, 9m), Item(2, 5, 20m)]));

        await act.Should().ThrowAsync<ConflictException>();

        _uow.RolledBack.Should().BeTrue();
        _uow.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Returns_the_original_sale_when_the_key_is_replayed()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);
        var sale = Sale([Item(1, 1, 30m)]);

        var first = await _service.ProcessSaleAsync(sale);
        var replay = await _service.ProcessSaleAsync(sale);

        replay.TransactionId.Should().Be(first.TransactionId);
        replay.WasReplay.Should().BeTrue();

        // The double tap must not sell twice.
        _uow.TransactionsFake.Saved.Should().HaveCount(1);
        _uow.ProductsFake[1].StockQuantity.Should().Be(9);
    }

    [Fact]
    public async Task Recalculates_the_balance_on_a_credit_sale()
    {
        _uow.ProductsFake.Add(id: 1, name: "Bigas", price: 60m, stock: 10);
        _uow.CustomersFake.Add(id: 7, name: "Aling Rosa");

        await _service.ProcessSaleAsync(
            Sale([Item(1, 1, 60m)], paymentType: "Credit", customerId: 7));

        _uow.CustomersFake.Recalculated.Should().ContainSingle().Which.Should().Be(7);
    }

    [Fact]
    public async Task Rejects_an_empty_cart()
    {
        var act = () => _service.ProcessSaleAsync(Sale([]));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task Rejects_utang_with_nobody_to_owe_it()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var act = () => _service.ProcessSaleAsync(
            Sale([Item(1, 1, 30m)], paymentType: "Credit", customerId: null));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task Rejects_an_unknown_payment_type()
    {
        var act = () => _service.ProcessSaleAsync(Sale([], paymentType: "GCash"));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task Reports_a_product_that_does_not_exist()
    {
        var act = () => _service.ProcessSaleAsync(Sale([Item(404, 1, 10m)]));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Reports_a_customer_that_does_not_exist()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var act = () => _service.ProcessSaleAsync(
            Sale([Item(1, 1, 30m)], paymentType: "Credit", customerId: 999));

        await act.Should().ThrowAsync<NotFoundException>();
        _uow.RolledBack.Should().BeTrue();
    }

    private static CheckoutItemDto Item(int productId, int quantity, decimal unitPrice)
        => new(productId, quantity, unitPrice);

    private static CheckoutDto Sale(
        IEnumerable<CheckoutItemDto> items,
        string paymentType = "Cash",
        int? customerId = null,
        string idempotencyKey = "test-key-0001")
        => new(paymentType, customerId, items, idempotencyKey);
}
