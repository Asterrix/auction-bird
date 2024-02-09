using LanguageExt;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Caching;

public interface ICachingService
{
    Task<Option<T>> RetrieveDataFromCache<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task StoreDataInCache<T>(string key, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default) where T : class;

    Task RemoveDataFromCache(string key, CancellationToken cancellationToken = default);
}