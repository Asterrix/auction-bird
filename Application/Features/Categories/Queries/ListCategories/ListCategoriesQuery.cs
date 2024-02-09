using Application.Caching;
using Application.Features.Categories.Mapper;
using Domain.Categories;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.Categories.Queries.ListCategories;

public record ListCategoriesQuery : IRequest<List<ParentCategory>>;

public class ListCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    ICachingService cachingService) : IRequestHandler<ListCategoriesQuery, List<ParentCategory>>,
    ICacheableCqrsService<List<ParentCategory>>
{
    public async Task<List<ParentCategory>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "categories_list";

        Option<List<ParentCategory>> cachedCategories = await RetrieveDataFromCache(cacheKey, cancellationToken);

        return await cachedCategories.MatchAsync(
            Some: categories => categories,
            None: async () => await RetrieveAndStoreDataInCache(cacheKey, cancellationToken)
        );
    }

    public async Task<Option<List<ParentCategory>>> RetrieveDataFromCache(string key, CancellationToken cancellationToken = default)
    {
        return await cachingService.RetrieveDataFromCache<List<ParentCategory>>(key, cancellationToken);
    }

    public async Task<List<ParentCategory>> RetrieveAndStoreDataInCache(string key, CancellationToken cancellationToken = default)
    {
        List<Category> categories = await categoryRepository.ListAllAsync(cancellationToken);
        List<ParentCategory> mappedCategories = CategoryMapper.MapToCategoryList(categories);

        await StoreDataInCache(key, mappedCategories, cancellationToken);

        return mappedCategories;
    }

    public async Task StoreDataInCache(string key, List<ParentCategory> data, CancellationToken cancellationToken = default)
    {
        TimeSpan cacheExpiration = TimeSpan.FromMinutes(5);

        DistributedCacheEntryOptions options = new CacheOptionsBuilder()
            .WithKey(key)
            .WithAbsoluteExpirationRelativeToNow(cacheExpiration)
            .Build();

        await cachingService.StoreDataInCache(key, data, options, cancellationToken);
    }
}