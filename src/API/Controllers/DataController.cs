using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly ItemService _itemService;

    public DataController(ItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost]
    public async Task<IActionResult> SaveData([FromBody] List<Dictionary<string, string>> input)
    {
        await _itemService.SaveDataAsync(input);
        return Ok(new { message = "Data saved successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetData([FromQuery] int? minCode, [FromQuery] int? maxCode, [FromQuery] string? valueContains)
    {
        var result = await _itemService.GetDataAsync(minCode, maxCode, valueContains);
        return Ok(result);
    }
}
