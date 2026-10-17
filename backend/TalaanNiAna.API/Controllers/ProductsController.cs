using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetProductsAsync(search, category, page, pageSize);
        return Ok(result);
    }

    // Literal routes must be registered before "{id}", or routing tries to parse
    // them as an int and returns 400 every time.
    [HttpGet("quick-items")]
    public async Task<IActionResult> GetQuickItems()
        => Ok(await _service.GetQuickItemsAsync());

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
        => Ok(await _service.GetCategoriesAsync());

    /// <summary>Scanner lookup at the till. 404 means the code is not on file yet.</summary>
    [HttpGet("barcode/{code}")]
    public async Task<IActionResult> GetByBarcode(string code)
    {
        var product = await _service.GetByBarcodeAsync(code);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
