using System.Text;
using Application.Caching;
using LanguageExt;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Caching;

public sealed class CachingService(IDistributedCache distributedCache) : ICachingService
{
    public async Task<Option<T>> RetrieveDataFromCache<T>(
        string key,
        CancellationToken cancellationToken = default) where T : class
    {
        Option<string> cachedData = await distributedCache.GetStringAsync(key, cancellationToken);

        return cachedData.Match(
            data => JsonConvert.DeserializeObject<T>(data),
            () => Option<T>.None
        );
    }

    public async Task StoreDataInCache<T>(
        string key,
        T data,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        string serializedData = JsonConvert.SerializeObject(data);
        byte[] serializedDataBytes = Encoding.UTF8.GetBytes(serializedData);

        await distributedCache.SetAsync(key, serializedDataBytes, options, cancellationToken);
    }

    public async Task RemoveDataFromCache(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}