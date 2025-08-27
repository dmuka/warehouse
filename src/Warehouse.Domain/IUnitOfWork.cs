namespace Warehouse.Domain;

public interface IUnitOfWork
{
    IWarehouseDbContext Context { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);
    
    Task RollbackAsync(CancellationToken cancellationToken);
}