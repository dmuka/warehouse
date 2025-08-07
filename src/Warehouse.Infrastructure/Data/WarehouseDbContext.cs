using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    
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