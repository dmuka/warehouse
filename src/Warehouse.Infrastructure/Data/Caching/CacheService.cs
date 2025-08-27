using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core;

namespace Warehouse.Infrastructure.Data.Caching;

public class CacheService(
    IMemoryCache cache, 
    ICacheKeyTracker keyTracker,
    ILogger<CacheService> logger) : ICacheService
{
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    
    public async Task<TEntity> GetOrCreateAsync<TEntity>(
        string cacheKey, 
        Func<Task<TEntity>> factory, 
        TimeSpan? expiration = null,
        IEnumerable<Guid>? relatedEntityIds = null)
    {
        await CacheLock.WaitAsync();
    
        try 
        {
            if (relatedEntityIds != null)
            {
                foreach (var entityId in relatedEntityIds)
                {
                    keyTracker.AddKeyForEntity<TEntity>(entityId, cacheKey);
                }
            }
        
            if (cache.TryGetValue(cacheKey, out TEntity? cachedData))
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                
                return cachedData;
            }

            var data = await factory();
            logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
            
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            cache.Set(cacheKey, data, options);
        
            return data;
        } 
        finally
        {
            CacheLock.Release();
        }
    }
    
    public void Remove(string cacheKey) => cache.Remove(cacheKey);
    
    public void RemoveAllForEntity<TEntity>(TypedId id)
    {
        foreach (var key in keyTracker.GetKeysForEntity<TEntity>(id))
        {
            cache.Remove(key);
        }
        
        keyTracker.ClearKeysForEntity<TEntity>(id);
    }
}