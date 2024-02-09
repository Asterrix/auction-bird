using LanguageExt;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Caching;

public interface ICachingService
{
    Task<Option<T>> RetrieveDataFromCache<T>(
        string key,
        CancellationToken cancellationToken = default) where T : class;
    Task<Option<T>> RetrieveDataFromCache<T>(
        string key,
        JsonSerializerSettings jsonSerializerSettings,  
        CancellationToken cancellationToken = default) where T : class;

    Task StoreDataInCache<T>(
        string key,
        T data,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default) where T : class;

    Task RemoveDataFromCache(
        string key,
        CancellationToken cancellationToken = default);
}