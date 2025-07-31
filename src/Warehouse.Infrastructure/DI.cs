using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Infrastructure.Data;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Infrastructure.Data.Repositories;

namespace Warehouse.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabase(configuration)
            .AddHealthChecks(configuration);


    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<WarehouseDbContext>(
            options => options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("Warehouse.Presentation")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IRepository<Resource>, Repository<Resource, ResourceDto>>();        
        services.AddScoped<IResourceRepository, ResourceRepository>();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database"));

        return services;
    }
}