using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;
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
        
        services.AddScoped<IRepository<Unit>, Repository<Unit, UnitDto>>();        
        services.AddScoped<IUnitRepository, UnitRepository>();
        
        services.AddScoped<IRepository<Unit>, Repository<Unit, UnitDto>>();        
        services.AddScoped<IUnitRepository, UnitRepository>();
        
        services.AddScoped<IRepository<Client>, Repository<Client, ClientDto>>();        
        services.AddScoped<IClientRepository, ClientRepository>();
        
        services.AddScoped<IRepository<Balance>, Repository<Balance, BalanceDto>>();        
        services.AddScoped<IBalanceRepository, BalanceRepository>();

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