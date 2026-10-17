using FluentAssertions;
using TalaanNiAna.Application.Common.Exceptions;
using TalaanNiAna.Tests.Fakes;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class ProductBarcodeTests
{
    private readonly FakeProductRepository _products = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly ProductService _service;

    public ProductBarcodeTests()
    {
        _service = new ProductService(_products, _stores);
    }

    [Fact]
    public async Task Finds_a_product_by_its_code()
    {
        _products.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, barcode: "4800016641107");

        var found = await _service.GetByBarcodeAsync("4800016641107");

        found!.Name.Should().Be("Coke Mismo");
    }

    [Fact]
    public async Task Ignores_surrounding_whitespace_from_the_scanner()
    {
        _products.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, barcode: "4800016641107");

        var found = await _service.GetByBarcodeAsync("  4800016641107\r");

        found.Should().NotBeNull();
    }

    [Fact]
    public async Task Returns_nothing_for_an_unknown_code()
    {
        var found = await _service.GetByBarcodeAsync("0000000000000");

        found.Should().BeNull();
    }

    [Fact]
    public async Task Rejects_a_code_another_product_already_uses()
    {
        _products.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, barcode: "4800016641107");

        var act = () => _service.CreateAsync(new CreateProductDto(
            "Royal Mismo", "Inumin", Price: 30m, StockQuantity: 5, Barcode: "4800016641107"));

        await act.Should().ThrowAsync<ConflictException>()
            .Where(e => e.Code == "duplicate_barcode");
    }

    [Fact]
    public async Task Lets_a_product_keep_its_own_code_on_edit()
    {
        _products.Add(id: 1, name: "Coke Mismo", price: 30m, stock: 10, barcode: "4800016641107");

        var updated = await _service.UpdateAsync(1, new UpdateProductDto(
            "Coke Mismo 290ml", "Inumin", Price: 32m, StockQuantity: 10, Barcode: "4800016641107"));

        updated!.Barcode.Should().Be("4800016641107");
        updated.Name.Should().Be("Coke Mismo 290ml");
    }

    [Fact]
    public async Task An_empty_code_clears_the_barcode()
    {
        _products.Add(id: 1, name: "Asin", price: 5m, stock: 10, barcode: "ASIN-1KG");

        var updated = await _service.UpdateAsync(1, new UpdateProductDto(
            "Asin", "Pampalasa", Price: 5m, StockQuantity: 10, Barcode: ""));

        updated!.Barcode.Should().BeNull();
    }

    [Fact]
    public async Task Editing_a_product_keeps_its_cost_price()
    {
        _products.Add(id: 1, name: "Asin", price: 5m, stock: 10, cost: 3m);

        var updated = await _service.UpdateAsync(1, new UpdateProductDto(
            "Asin", "Pampalasa", Price: 6m, StockQuantity: 10));

        updated!.CostPrice.Should().Be(3m);
    }
}
