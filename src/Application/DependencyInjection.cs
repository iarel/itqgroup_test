using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ComputeApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ItemService>();
        return services;
    }
}
