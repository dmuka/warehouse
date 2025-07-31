using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Data;

public class UnitOfWork(WarehouseDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

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

    public async Task Dispose()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
        await context.DisposeAsync();
    }
}