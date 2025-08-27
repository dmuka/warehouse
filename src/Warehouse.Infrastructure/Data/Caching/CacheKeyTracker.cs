using System.Collections.Concurrent;
using Warehouse.Application.Abstractions.Cache;

namespace Warehouse.Infrastructure.Data.Caching;

public class CacheKeyTracker : ICacheKeyTracker
{
    private readonly ConcurrentDictionary<string, List<string>> _entityKeys = new();
    
    public void AddKeyForEntity<TEntity>(Guid id, string cacheKey)
    {
        var entityName = typeof(TEntity).Name.ToLowerInvariant();
        var compositeKey = $"{entityName}:{id}";
        
        _entityKeys.AddOrUpdate(
            compositeKey,
            [cacheKey],
            (_, existingKeys) =>
            {
                if (!existingKeys.Contains(cacheKey)) existingKeys.Add(cacheKey);
                
                return existingKeys;
            });
    }
    
    public IEnumerable<string> GetKeysForEntity<TEntity>(Guid id)
    {
        var entityName = typeof(TEntity).Name.ToLowerInvariant();
        var compositeKey = $"{entityName}:{id}";
        
        return _entityKeys.TryGetValue(compositeKey, out var keys) 
            ? keys 
            : Enumerable.Empty<string>();
    }
    
    public void ClearKeysForEntity<TEntity>(Guid id)
    {
        var entityName = typeof(TEntity).Name.ToLowerInvariant();
        var compositeKey = $"{entityName}:{id}";
        _entityKeys.TryRemove(compositeKey, out _);
    }
}