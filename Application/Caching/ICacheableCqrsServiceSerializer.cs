using LanguageExt;
using Newtonsoft.Json;

namespace Application.Caching;

public interface ICacheableCqrsServiceSerializer<T> where T : class
{
    Task<Option<T>> RetrieveDataFromCache(
        string key,
        JsonSerializerSettings serializer,
        CancellationToken cancellationToken = default);
    Task StoreDataInCache(
        string key,
        T data,
        CancellationToken cancellationToken = default);
}