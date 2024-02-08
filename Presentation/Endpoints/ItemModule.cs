using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.ListItems;
using Application.Pagination;
using Carter;
using Infrastructure.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

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
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        string cacheKey = $"items_list_{pageNumber}_{pageSize}";
        DistributedCacheEntryOptions cacheOptions = new CacheOptionsBuilder()
            .WithKey(cacheKey)
            .WithAbsoluteExpirationRelativeToNow(TimeSpan.FromMinutes(5))
            .Build();
        
        Page<ItemSummary>? cachedItems = await cachingService.GetAsync<Page<ItemSummary>>(cacheKey);
        if (cachedItems is not null)
        {
            // Because the cached item does not contain the reference to Pageable object,
            // we need to create a new one in order for properties TotalPages and IsLastPage to be calculated correctly.
            // Else NullReferenceException will be thrown when trying to access these properties.
            cachedItems = new Page<ItemSummary>(cachedItems.Elements, Pageable.Of(pageNumber, pageSize), cachedItems.TotalElements);
            return Results.Ok(cachedItems);
        }

        Pageable pageable = Pageable.Of(pageNumber, pageSize);
        Page<ItemSummary> items = await sender.Send(new ListItemsQuery(pageable));
        await cachingService.SetAsync(cacheKey, items, cacheOptions);

        return Results.Ok(items);
    }
}