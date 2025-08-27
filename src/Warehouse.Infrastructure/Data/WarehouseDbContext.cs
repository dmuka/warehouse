using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options), IWarehouseDbContext
{
    public DbSet<Resource> Resources { get; set; }

    public DbSet<Unit> Units { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }
    
    public async Task<IList<TEntity>> GetFromRawSqlAsync<TEntity>(
        string sql, 
        IEnumerable<object>? parameters, 
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var paramsArray = parameters?.ToArray() ?? [];
        
        return await Database
            .SqlQueryRaw<TEntity>(sql, paramsArray)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ResourceId>();
        modelBuilder.Ignore<UnitId>();
        modelBuilder.Ignore<ClientId>();
        modelBuilder.Ignore<BalanceId>();
        modelBuilder.Ignore<ReceiptId>();
        modelBuilder.Ignore<ShipmentId>();
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
    }
}