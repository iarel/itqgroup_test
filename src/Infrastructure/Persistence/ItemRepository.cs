using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAllAsync(List<Item> items)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE items RESTART IDENTITY");
            _context.Items.AddRange(items);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Item>> GetAllAsync(int? minCode, int? maxCode, string? valueContains)
    {
        var query = _context.Items.AsQueryable();

        if (minCode.HasValue)
            query = query.Where(i => i.Code >= minCode.Value);
        
        if (maxCode.HasValue)
            query = query.Where(i => i.Code <= maxCode.Value);
        
        if (!string.IsNullOrEmpty(valueContains))
            query = query.Where(i => i.Value.Contains(valueContains));

        return await query.OrderBy(i => i.Code).ToListAsync();
    }
}
