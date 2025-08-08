using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Warehouse.Core;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Data.Repositories;

public class Repository<TEntity>(WarehouseDbContext context) : IRepository<TEntity>
    where TEntity : Entity
{
    public IQueryable<TEntity> GetQueryable()
    {
        return context.Set<TEntity>().AsQueryable();
    }

    public async Task<IList<TEntity>> QueryableToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetListAsync(
        CancellationToken cancellationToken = default,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
    {
        var query = context.Set<TEntity>().AsNoTracking();
    
        if (include != null)
        {
            query = include(query);
        }
    
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TypedId id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        return entity;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(condition, cancellationToken);
        
        return entity;
    }

    public async Task<bool> ExistsByIdAsync(TypedId id, CancellationToken cancellationToken = default) 
    {
        return await context.Set<TEntity>().FindAsync([id], cancellationToken) is not null;
    }
    
    public async Task<IList<TEntity>> GetFromRawSqlAsync(
        string sql, 
        List<object>? parameters, 
        CancellationToken cancellationToken = default)
    {
        return await context.Database
            .SqlQueryRaw<TEntity>(sql, parameters?.ToArray() ?? [])
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public void Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
    }

    public async Task Delete(TypedId entityId)
    {
        var entity = await context.Set<TEntity>().FindAsync(entityId);
        if (entity is not null)
        {
            context.Set<TEntity>().Remove(entity);
        }
    }

    public async Task<IList<TEntity>> GetEntitiesByCondition(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        var entities = await context.Set<TEntity>()
            .AsNoTracking()
            .Where(condition)
            .ToListAsync(cancellationToken);
        
        return entities;
    }

    public void ClearChangeTracker()
    {
        context.ChangeTracker.Clear();
    }
}