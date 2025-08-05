using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation;

public static class DI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddCors()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddControllers();

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowWarehouseClientApp",
                policy => policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        return services;
    }
}