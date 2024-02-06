using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

public sealed class CacheOptionsBuilder
{
    private string _key = string.Empty;
    private readonly DistributedCacheEntryOptions _options = new();

    /// <summary>
    /// Set the key for the cache entry.
    /// </summary>
    public CacheOptionsBuilder WithKey(in string key)
    {
        _key = key;
        return this;
    }

    /// <summary>
    /// Set the absolute expiration for the cache entry.
    /// </summary>
    public CacheOptionsBuilder WithAbsoluteExpiration(in DateTimeOffset absoluteExpiration)
    {
        _options.AbsoluteExpiration = absoluteExpiration;
        return this;
    }

    /// <summary>
    /// Set the absolute expiration for the cache entry relative to now.
    /// </summary>
    public CacheOptionsBuilder WithAbsoluteExpirationRelativeToNow(in TimeSpan absoluteExpirationRelativeToNow)
    {
        _options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        return this;
    }

    /// <summary>
    /// Set the sliding expiration for the cache entry.
    /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
    /// </summary>
    public CacheOptionsBuilder WithSlidingExpiration(in TimeSpan slidingExpiration)
    {
        _options.SlidingExpiration = slidingExpiration;
        return this;
    }

    /// <summary>
    /// Build the DistributedCacheEntryOptions.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the key is empty</exception>
    /// <returns>DistributedCacheEntryOptions</returns>
    public DistributedCacheEntryOptions Build()
    {
        if (_key == string.Empty)
        {
            throw new ArgumentException("Key cannot be empty");
        }

        return _options;
    }
}