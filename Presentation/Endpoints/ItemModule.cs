using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.ListItems;
using Application.Pagination;
using Carter;
using Infrastructure.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Presentation.JsonConverters;

namespace Presentation.Endpoints;

public sealed class ItemModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("items", ListItems);
    }

    private static async Task<IResult> ListItems(
        ISender sender,
        ICachingService cachingService,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        string cacheKey = $"items_list_{page}_{size}";

        Page<ItemSummary>? cachedItems = await cachingService.GetAsync<Page<ItemSummary>>(cacheKey, PageConverter<ItemSummary>.GetSettings());
        if (cachedItems is not null)
        {
            return Results.Ok(cachedItems);
        }

        Pageable pageable = Pageable.Of(page, size);
        Page<ItemSummary> items = await sender.Send(new ListItemsQuery(pageable));

        DistributedCacheEntryOptions cacheOptions = new CacheOptionsBuilder()
            .WithKey(cacheKey)
            .WithAbsoluteExpirationRelativeToNow(items.IsLastPage ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(10))
            .Build();

        await cachingService.SetAsync(cacheKey, items, cacheOptions);

        return Results.Ok(items);
    }
}