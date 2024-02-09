using Application.Caching;
using Application.Features.Items.Mapper;
using Application.JsonConverters;
using Application.Pagination;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Features.Items.Queries.ListItems;

public record ListItemsQuery(Pageable Pageable) : IRequest<Page<ItemSummary>>;

public sealed class ListItemsQueryHandler(IItemRepository itemRepository, ICachingService cachingService)
    : IRequestHandler<ListItemsQuery, Page<ItemSummary>>, ICacheableCqrsServiceSerializer<Page<ItemSummary>>
{
    public async Task<Page<ItemSummary>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        string key = $"list_items:p={request.Pageable.Page}:s={request.Pageable.Size}";
        JsonSerializerSettings serializer = PageConverter<ItemSummary>.GetSettings();

        Option<Page<ItemSummary>> cachedData = await RetrieveDataFromCache(key, serializer, cancellationToken);

        return await cachedData.MatchAsync(
            itemPage => itemPage,
            async () =>
            {
                Page<ItemSummary> items = await itemRepository.ListAllAsync(request.Pageable, cancellationToken);
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
        return await cachingService.RetrieveDataFromCache<Page<ItemSummary>>(key, serializer, cancellationToken);
    }

    public async Task StoreDataInCache(string key, Page<ItemSummary> data,
        CancellationToken cancellationToken = default)
    {
        TimeSpan expiration = TimeSpan.FromMinutes(5);

        DistributedCacheEntryOptions options = new CacheOptionsBuilder()
            .WithKey(key)
            .WithAbsoluteExpirationRelativeToNow(expiration)
            .Build();

        await cachingService.StoreDataInCache(key, data, options, cancellationToken);
    }
}