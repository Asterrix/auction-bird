namespace Application.Caching;

public interface ICacheKeyBuilder<in TRequest>
{
    string BuildKey(TRequest request);
}