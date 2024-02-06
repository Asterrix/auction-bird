using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions cacheEntryOptions,
        CancellationToken cancellationToken = default)
        where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}