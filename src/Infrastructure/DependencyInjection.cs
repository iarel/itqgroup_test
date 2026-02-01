using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class ComputeInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        // Repositories
        services.AddScoped<IItemRepository, ItemRepository>();

        // MongoDB
        services.AddSingleton<IMongoClient>(sp => 
            new MongoClient(configuration.GetConnectionString("MongoConnection")));

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}
