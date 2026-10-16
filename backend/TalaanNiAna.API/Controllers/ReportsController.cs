using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ZReportService _zReport;

    public ReportsController(ZReportService zReport)
    {
        _zReport = zReport;
    }

    /// <summary>End-of-day close. Omit the date for today in the store's timezone.</summary>
    [HttpGet("z")]
    public async Task<IActionResult> GetZReport([FromQuery] DateOnly? date)
        => Ok(await _zReport.GetAsync(date));
}
