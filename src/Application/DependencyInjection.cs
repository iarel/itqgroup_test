using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ComputeApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ItemService>();
        services.AddValidatorsFromAssembly(typeof(ComputeApplication).Assembly);
        return services;
    }
}
