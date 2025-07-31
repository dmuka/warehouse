using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Warehouse.Core;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Data.Repositories;

public class Repository<TEntity, TEntityDto>(WarehouseDbContext context) : IRepository<TEntity>
    where TEntity : AggregateRoot
    where TEntityDto : Dto, new()
{
    public IQueryable<TEntity> GetQueryable()
    {
        return context.Set<TEntity>().AsQueryable();
    }

    public async Task<IList<TEntity>> QueryableToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await context.Set<TEntityDto>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var entities = dtos.Select(dto => (TEntity)dto.ToEntity());
        
        return entities.ToList();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dto = await context.Set<TEntityDto>()
            .AsNoTracking()
            .FirstOrDefaultAsync(dto => dto.Id == id, cancellationToken);
        
        return dto is not null ? (TEntity)dto.ToEntity() : null;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        var conditionDto = Dto.EntityToDtoMapper<TEntity, TEntityDto>.ConvertCondition(condition);
        var dto = await context.Set<TEntityDto>()
            .AsNoTracking()
            .FirstOrDefaultAsync(conditionDto, cancellationToken);
        
        return dto is not null ? (TEntity)dto.ToEntity() : null;;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) 
    {
        return await context.Set<TEntityDto>().FindAsync([id], cancellationToken) is not null;
    }

    public async Task<IList<TEntity>> GetFromRawSqlAsync(
        string sql, 
        List<object>? parameters, 
        CancellationToken cancellationToken = default)
    {
        var dtos = await context.Set<TEntityDto>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var entities = dtos.Select(dto => (TEntity)dto.ToEntity());
        
        return entities.ToList();
    }

    public void Add(TEntity entity)
    {
        var dto = new TEntityDto();
        dto.ToDto(entity);
        
        context.Set<TEntityDto>().Add(dto);
    }

    public void Update(TEntity entity)
    {
        var dto = new TEntityDto();
        dto.ToDto(entity);
        
        context.Set<TEntityDto>().Update(dto);
    }

    public async Task Delete(Guid entityId)
    {
        var entity = await context.Set<TEntityDto>().FindAsync(entityId);
        if (entity is not null)
        {
            context.Set<TEntityDto>().Remove(entity);
        }
    }

    public async Task<IList<TEntity>> GetEntitiesByCondition(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        var conditionDto = Dto.EntityToDtoMapper<TEntity, TEntityDto>.ConvertCondition(condition);
        
        var dtos = await context.Set<TEntityDto>()
            .AsNoTracking()
            .Where(conditionDto)
            .ToListAsync(cancellationToken);
        var entities = dtos.Select(dto => (TEntity)dto.ToEntity());
        
        return entities.ToList();
    }

    public void ClearChangeTracker()
    {
        context.ChangeTracker.Clear();
    }
}