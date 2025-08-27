using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation;

public static class DI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddCors()
            .AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        })
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddControllers(options => options.Filters.Add<ResultFilter>());
            // .AddJsonOptions(options => 
            // {
            //     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //     options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //     options.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
            //     options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //     options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            // });

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