using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation;

public static class DI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddControllers();

        return services;
    }
}