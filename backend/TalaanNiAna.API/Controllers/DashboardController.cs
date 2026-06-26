using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _service.GetSummaryAsync();
        return Ok(summary);
    }
}