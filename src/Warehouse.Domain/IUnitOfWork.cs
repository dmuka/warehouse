using Warehouse.Core;

namespace Warehouse.Domain;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);
    
    Task RollbackAsync(CancellationToken cancellationToken);
    void TrackDomainEvents(Entity aggregate);
    Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default);
    
    Task Dispose();
}