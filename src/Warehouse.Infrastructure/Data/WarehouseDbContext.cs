using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<ResourceDto> Resources { get; set; }
    public DbSet<UnitDto> Units { get; set; }
    public DbSet<ClientDto> Clients { get; set; }
    public DbSet<BalanceDto> Balances { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
    }
}