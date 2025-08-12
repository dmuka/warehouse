using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Warehouse.Core;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Data;

public class UnitOfWork(
    WarehouseDbContext context, 
    IMediator mediator, 
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private readonly List<IDomainEvent> _domainEvents = [];

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await DispatchDomainEventsAsync(cancellationToken);
                await _transaction.DisposeAsync();
            }
        }
        catch (Exception exception)
        {
            logger.LogError("Unit of work exception: {Message}", exception.InnerException);
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction is null) return;
        
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
    }

    public void TrackDomainEvents(Entity aggregate)
    {
        _domainEvents.AddRange(aggregate.DomainEvents);
        aggregate.ClearDomainEvents();
    }

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        while (_domainEvents.Count != 0)
        {
            var eventsToDispatch = _domainEvents.ToList();
            _domainEvents.Clear();
            
            foreach (var domainEvent in eventsToDispatch)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }

    public async Task Dispose()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
        await context.DisposeAsync();
    }
}