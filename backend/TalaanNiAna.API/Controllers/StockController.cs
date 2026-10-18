using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly StockService _service;
    private readonly RepackService _repack;

    public StockController(StockService service, RepackService repack)
    {
        _service = service;
        _repack = repack;
    }

    /// <summary>Records a delivery, a spoilage, a return, or a count correction.</summary>
    [HttpPost("movements")]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Record([FromBody] RecordStockDto dto)
        => Ok(await _service.RecordAsync(dto));

    /// <summary>Opens packs into tingi, moving both sides in one transaction.</summary>
    [HttpPost("repack")]
    [ProducesResponseType(typeof(RepackResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Repack([FromBody] RepackDto dto)
        => Ok(await _repack.RepackAsync(dto));

    /// <summary>Everything that has moved this product, newest first.</summary>
    [HttpGet("movements/{productId:int}")]
    public async Task<IActionResult> History(int productId, [FromQuery] int count = 50)
        => Ok(await _service.GetHistoryAsync(productId, Math.Clamp(count, 1, 200)));

    /// <summary>Stock lost rather than sold. Defaults to the last 30 days.</summary>
    [HttpGet("shrinkage")]
    public async Task<IActionResult> Shrinkage([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        => Ok(await _service.GetShrinkageAsync(from, to));
}
