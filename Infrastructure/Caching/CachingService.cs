using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Caching;

public sealed class CachingService(IDistributedCache distributedCache) : ICachingService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? value = await distributedCache.GetStringAsync(key, cancellationToken);

        return value is not null ? JsonConvert.DeserializeObject<T>(value) : null;
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions cacheEntryOptions,
        CancellationToken cancellationToken = default) where T : class
    {
        string serializedValue = JsonConvert.SerializeObject(value);
        byte[] serializedBytes = Encoding.UTF8.GetBytes(serializedValue);

        await distributedCache.SetAsync(key, serializedBytes, cacheEntryOptions, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}