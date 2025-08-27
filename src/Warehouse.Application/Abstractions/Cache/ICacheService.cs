using Warehouse.Core;

namespace Warehouse.Application.Abstractions.Cache;

public interface ICacheService
{
    Task<TEntity> GetOrCreateAsync<TEntity>(string cacheKey, Func<Task<TEntity>> factory, TimeSpan? expiration = null, IEnumerable<Guid>? relatedEntityIds = null);
    void Remove(string cacheKey);
    void RemoveAllForEntity<TEntity>(TypedId id);
}