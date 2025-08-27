using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;
using Warehouse.Infrastructure.Data;
using Warehouse.Infrastructure.Data.Caching;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Infrastructure.Data.Repositories;

namespace Warehouse.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabase(configuration)
            .AddCache()
            .AddHealthChecks(configuration);


    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'Database' is not configured.");
        }

        services.AddDbContextPool<WarehouseDbContext>(options => 
        {
            options.UseSqlServer(connectionString, sqlOptions => 
            {
                sqlOptions.MigrationsAssembly("Warehouse.Presentation");
            });
        }, poolSize: 128);
        
        services.AddHostedService<WarmupService>();

        services.AddScoped<IWarehouseDbContext, WarehouseDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IRepository<Resource>, Repository<Resource>>();        
        services.AddScoped<IResourceRepository, ResourceRepository>();
        
        services.AddScoped<IRepository<Unit>, Repository<Unit>>();        
        services.AddScoped<IUnitRepository, UnitRepository>();
        
        services.AddScoped<IRepository<Client>, Repository<Client>>();        
        services.AddScoped<IClientRepository, ClientRepository>();
        
        services.AddScoped<IRepository<Receipt>, Repository<Receipt>>();        
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        
        services.AddScoped<IRepository<Shipment>, Repository<Shipment>>();        
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        
        services.AddScoped<IRepository<BalanceDto>, Repository<BalanceDto>>();       
        services.AddScoped<IBalanceRepository, BalanceRepository>(); 

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        
        services.AddSingleton<ICacheKeyGenerator, CacheKeyGenerator>();
        
        services.AddSingleton<ICacheService, CacheService>();        
        services.AddSingleton<ICacheKeyTracker, CacheKeyTracker>();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null) throw new InvalidOperationException("The \"Database\" configuration key is missing or empty.");
        
        services
            .AddHealthChecks()
            .AddSqlServer(connectionString);

        return services;
    }
}