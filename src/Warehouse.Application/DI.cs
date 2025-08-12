using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DI).Assembly);
        });
        
        return services;
    }
}
