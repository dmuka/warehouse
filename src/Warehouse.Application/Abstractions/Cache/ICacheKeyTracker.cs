namespace Warehouse.Application.Abstractions.Cache;

public interface ICacheKeyTracker
{
    void AddKeyForEntity<TEntity>(Guid entityId, string cacheKey);
    IEnumerable<string> GetKeysForEntity<TEntity>(Guid id);
    void ClearKeysForEntity<TEntity>(Guid id);
}