using Application.Common.Models;
using FluentValidation;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Services;

public class ItemService
{
    private readonly IItemRepository _repository;
    private readonly ICacheService _cache;
    private readonly IValidator<List<Dictionary<string, string>>> _validator;

    public ItemService(IItemRepository repository, ICacheService cache, IValidator<List<Dictionary<string, string>>> validator)
    {
        _repository = repository;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Result> SaveDataAsync(List<Dictionary<string, string>> input)
    {
        var validationResult = await _validator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

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

        var sortedItems = items.OrderBy(i => i.Code).ToList();
        await _repository.SaveAllAsync(sortedItems);

        // Invalidate cache by updating global version
        await _cache.SetStringAsync("GlobalDataVersion", DateTime.UtcNow.Ticks.ToString());

        return Result.Success();
    }

    public async Task<Result<List<ItemDto>>> GetDataAsync(int? minCode, int? maxCode, string? valueContains)
    {
        var version = await _cache.GetStringAsync("GlobalDataVersion") ?? "0";
        var cacheKey = $"data_{version}_{minCode}_{maxCode}_{valueContains}";

        var cachedItems = await _cache.GetAsync<List<ItemDto>>(cacheKey);
        if (cachedItems != null)
        {
            return Result<List<ItemDto>>.Success(cachedItems);
        }

        var items = await _repository.GetAllAsync(minCode, maxCode, valueContains);
        var dtos = items.Select(i => new ItemDto { Id = i.Id, Code = i.Code, Value = i.Value }).ToList();

        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));

        return Result<List<ItemDto>>.Success(dtos);
    }
}
