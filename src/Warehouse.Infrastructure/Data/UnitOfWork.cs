using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Warehouse.Core;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Data;

public class UnitOfWork(
    IWarehouseDbContext context, 
    IMediator mediator,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private readonly List<IDomainEvent> _domainEvents = [];

    public IWarehouseDbContext Context { get; } = context;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction ??= await Context.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            CollectDomainEventsAsync();
            
            await Context.SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            
            await DispatchDomainEventsAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unit of work commit failed: {Message}", exception.Message);
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private void CollectDomainEventsAsync()
    {
        var domainEvents = Context.ChangeTracker
            .Entries<Entity>()
            .Where(entityEntry => entityEntry.Entity.DomainEvents.Count != 0)
            .SelectMany(entityEntry => entityEntry.Entity.DomainEvents)
            .ToList();
        
        foreach (var entry in Context.ChangeTracker.Entries<Entity>())
        {
            entry.Entity.ClearDomainEvents();
        }
        
        _domainEvents.AddRange(domainEvents);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction is null) return;
        
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction");
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
            _domainEvents.Clear();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var eventsToDispatch = _domainEvents.ToList();
        _domainEvents.Clear();

        foreach (var domainEvent in eventsToDispatch)
        {
            try
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish domain event: {EventType}", domainEvent.GetType().Name);
            }
        }
    }
}