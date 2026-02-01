namespace Application.Interfaces;

using System.Threading.Tasks;
using System;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null);
    Task<string?> GetStringAsync(string key);
}
