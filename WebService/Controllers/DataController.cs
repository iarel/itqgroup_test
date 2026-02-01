using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebService.Data;
using WebService.Entities;

namespace WebService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;

    public DataController(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpPost]
    public async Task<IActionResult> SaveData([FromBody] List<Dictionary<string, string>> input)
    {
        // Transform input
        var items = new List<Item>();
        foreach (var dict in input)
        {
            foreach (var kvp in dict)
            {
                if (int.TryParse(kvp.Key, out var code))
                {
                    items.Add(new Item { Code = code, Value = kvp.Value });
                }
            }
        }

        // Sort by Code
        var sortedItems = items.OrderBy(i => i.Code).ToList();
        
        // Re-assign IDs (ordinal number)? 
        // Requirement says "ordinal number", "code", "value". 
        // Relational DB auto-inc ID can serve as ordinal if inserted in order.
        // We will trust the database to generate sequential IDs.

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Clear table
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE items RESTART IDENTITY");

            // Save new data
            _context.Items.AddRange(sortedItems);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Invalidate cache by updating the global version
            await _cache.SetStringAsync("GlobalDataVersion", DateTime.UtcNow.Ticks.ToString());
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return Ok(new { message = "Data saved successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetData([FromQuery] int? minCode, [FromQuery] int? maxCode, [FromQuery] string? valueContains)
    {
        var version = await _cache.GetStringAsync("GlobalDataVersion") ?? "0";
        var cacheKey = $"data_{version}_{minCode}_{maxCode}_{valueContains}";
        
        // Check cache
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return Ok(JsonSerializer.Deserialize<List<Item>>(cachedData));
        }

        // Query DB
        var query = _context.Items.AsQueryable();

        if (minCode.HasValue)
            query = query.Where(i => i.Code >= minCode.Value);
        
        if (maxCode.HasValue)
            query = query.Where(i => i.Code <= maxCode.Value);
        
        if (!string.IsNullOrEmpty(valueContains))
            query = query.Where(i => i.Value.Contains(valueContains));

        var result = await query.OrderBy(i => i.Code).ToListAsync();

        // Cache result (5 minutes)
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(result);
    }
}
