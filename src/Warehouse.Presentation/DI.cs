using System.Text.Json;
using System.Text.Json.Serialization;
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
            .AddControllers()
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

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