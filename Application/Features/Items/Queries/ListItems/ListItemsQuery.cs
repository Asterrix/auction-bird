using Application.Caching;
using Application.Features.Items.Mapper;
using Application.Filtration;
using Application.JsonConverters;
using Application.Pagination;
using Application.Specification;
using Domain.Items;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Features.Items.Queries.ListItems;

public record ListItemsQuery(Pageable Pageable, string? Search) : IRequest<Page<ItemSummary>>;

public sealed class ListItemsQueryHandler : IRequestHandler<ListItemsQuery, Page<ItemSummary>>,
    ICacheableCqrsServiceSerializer<Page<ItemSummary>>
{
    private readonly ICacheKeyBuilder<ListItemsQuery> _cacheKeyBuilder;
    private readonly ICachingService _cachingService;
    private readonly IFilter<ListItemsQuery, ISpecification<Item>> _filter;
    private readonly IItemRepository _itemRepository;

    public ListItemsQueryHandler(
        IItemRepository itemRepository,
        ICachingService cachingService,
        ICacheKeyBuilder<ListItemsQuery> cacheKeyBuilder,
        IFilter<ListItemsQuery, ISpecification<Item>> filter)
    {
        _itemRepository = itemRepository;
        _cachingService = cachingService;
        _cacheKeyBuilder = cacheKeyBuilder;
        _filter = filter;
    }
    
    public async Task<Page<ItemSummary>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        string key = _cacheKeyBuilder.BuildKey(request);
        JsonSerializerSettings serializer = PageConverter<ItemSummary>.GetSettings();
        ISpecification<Item> filter = _filter.Filter(request, cancellationToken);

        Option<Page<ItemSummary>> cachedData = await RetrieveDataFromCache(key, serializer, cancellationToken);

        return await cachedData.MatchAsync(
            itemPage => itemPage,
            async () =>
            {
                Page<ItemSummary> items = await _itemRepository.ListAllAsync(request.Pageable, filter, cancellationToken);
                await StoreDataInCache(key, items, cancellationToken);

                return items;
            }
        );
    }

    public async Task<Option<Page<ItemSummary>>> RetrieveDataFromCache(
        string key,
        JsonSerializerSettings serializer,
        CancellationToken cancellationToken = default)
    {
        return await _cachingService.RetrieveDataFromCache<Page<ItemSummary>>(key, serializer, cancellationToken);
    }

    public async Task StoreDataInCache(string key, Page<ItemSummary> data,
        CancellationToken cancellationToken = default)
    {
        TimeSpan expiration = TimeSpan.FromMinutes(5);

        DistributedCacheEntryOptions options = new CacheOptionsBuilder()
            .WithKey(key)
            .WithAbsoluteExpirationRelativeToNow(expiration)
            .Build();

        await _cachingService.StoreDataInCache(key, data, options, cancellationToken);
    }
}