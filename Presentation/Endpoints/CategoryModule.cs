using Application.Features.Categories.Mapper;
using Application.Features.Categories.Queries.ListCategories;
using Carter;
using Domain.Categories;
using Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Presentation.Endpoints;

public sealed class CategoryModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("categories", ListCategories);
    }

    private static async Task<IResult> ListCategories(ISender sender, ICachingService cachingService)
    {
        const string cacheKey = "categories_list";
        DistributedCacheEntryOptions cacheOptions = new CacheOptionsBuilder()
            .WithKey(cacheKey)
            .WithAbsoluteExpirationRelativeToNow(TimeSpan.FromMinutes(5))
            .Build();

        List<ParentCategory>? cachedCategories = await cachingService.GetAsync<List<ParentCategory>>(cacheKey);

        if (cachedCategories is not null)
        {
            return Results.Ok(cachedCategories);
        }

        List<Category> categories = await sender.Send(new ListCategoriesQuery());
        List<ParentCategory> mappedCategories = CategoryMapper.MapToCategoryList(categories);

        await cachingService.SetAsync(cacheKey, mappedCategories, cacheOptions);
        return Results.Ok(mappedCategories);
    }
}