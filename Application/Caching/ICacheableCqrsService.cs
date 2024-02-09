using LanguageExt;

namespace Application.Caching;

public interface ICacheableCqrsService<T> where T : class
{
    Task<Option<T>> RetrieveDataFromCache(string key, CancellationToken cancellationToken = default);
    Task StoreDataInCache(string key, T data, CancellationToken cancellationToken = default);
}