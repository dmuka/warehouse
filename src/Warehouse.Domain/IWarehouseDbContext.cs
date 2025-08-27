using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain;

public interface IWarehouseDbContext
{
    DbSet<Balance> Balances { get; }
    DbSet<Unit> Units { get; }
    DbSet<Resource> Resources { get; }
    DbSet<Client> Clients { get; }
    DbSet<Shipment> Shipments { get; }
    DbSet<Receipt> Receipts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<IList<TEntity>> GetFromRawSqlAsync<TEntity>(string sql, IEnumerable<object>? parameters, CancellationToken cancellationToken = default) 
        where TEntity : class;

    ChangeTracker ChangeTracker { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    ValueTask DisposeAsync();
}