using Domain.Entities;

namespace Domain.Interfaces;

public interface IItemRepository
{
    Task SaveAllAsync(List<Item> items);
    Task<List<Item>> GetAllAsync(int? minCode, int? maxCode, string? valueContains);
}
