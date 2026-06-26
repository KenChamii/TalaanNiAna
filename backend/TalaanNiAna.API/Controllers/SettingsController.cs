using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly SettingsService _service;

    public SettingsController(SettingsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await _service.GetAsync();
        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] StoreSettingsDto dto)
    {
        var saved = await _service.SaveAsync(dto);
        return Ok(saved);
    }
}