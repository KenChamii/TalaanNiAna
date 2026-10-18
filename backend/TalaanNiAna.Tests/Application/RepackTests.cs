using FluentAssertions;
using TalaanNiAna.Application.Common.Exceptions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class RepackTests
{
    private readonly FakeUnitOfWork _uow = new();
    private readonly RepackService _service;

    public RepackTests()
    {
        _service = new RepackService(_uow);
    }

    /// <summary>A twelve-pack of shampoo, and the sachet it breaks into.</summary>
    private (Product pack, Product sachet) Shampoo(int packStock = 4, decimal packCost = 60m)
    {
        var pack = _uow.ProductsFake.Add(
            id: 1, name: "Palmolive 12s", price: 84m, stock: packStock, cost: packCost);

        var sachet = _uow.ProductsFake.Add(
            id: 2, name: "Palmolive Sachet", price: 7m, stock: 0);

        sachet.ParentProductId = pack.Id;
        sachet.UnitsPerParent = 12;

        return (pack, sachet);
    }

    [Fact]
    public async Task Opening_a_pack_moves_stock_both_ways()
    {
        var (pack, sachet) = Shampoo(packStock: 4);

        var result = await _service.RepackAsync(new RepackDto(TingiProductId: 2, ParentQuantity: 1));

        pack.StockQuantity.Should().Be(3);
        sachet.StockQuantity.Should().Be(12);
        result.TingiProduced.Should().Be(12);
    }

    [Fact]
    public async Task The_cost_per_piece_is_the_pack_split_by_what_it_yields()
    {
        var (_, sachet) = Shampoo(packCost: 60m);

        await _service.RepackAsync(new RepackDto(TingiProductId: 2));

        sachet.CostPrice.Should().Be(5m);   // 60 / 12
    }

    [Fact]
    public async Task Both_halves_are_written_to_the_ledger()
    {
        Shampoo();

        await _service.RepackAsync(new RepackDto(TingiProductId: 2, ParentQuantity: 2));

        var movements = _uow.StockMovementsFake.Movements;
        movements.Should().HaveCount(2);

        movements.Single(m => m.Reason == StockReason.RepackOut).Quantity.Should().Be(-2);
        movements.Single(m => m.Reason == StockReason.RepackIn).Quantity.Should().Be(24);
    }

    [Fact]
    public async Task Cannot_open_more_packs_than_there_are()
    {
        Shampoo(packStock: 1);

        var act = () => _service.RepackAsync(new RepackDto(TingiProductId: 2, ParentQuantity: 3));

        await act.Should().ThrowAsync<ConflictException>()
            .Where(e => e.Code == "insufficient_stock");
    }

    [Fact]
    public async Task A_product_with_no_parent_cannot_be_repacked()
    {
        _uow.ProductsFake.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10);

        var act = () => _service.RepackAsync(new RepackDto(TingiProductId: 1));

        await act.Should().ThrowAsync<RequestValidationException>();
    }

    [Fact]
    public async Task A_pack_with_no_recorded_cost_leaves_the_tingi_cost_alone()
    {
        var (_, sachet) = Shampoo(packCost: 0m);
        sachet.CostPrice = 4m;

        await _service.RepackAsync(new RepackDto(TingiProductId: 2));

        // Dividing an unknown cost would write a confident zero over a real figure.
        sachet.CostPrice.Should().Be(4m);
    }
}
