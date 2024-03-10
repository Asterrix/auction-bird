using Application.Caching;
using Application.Specification;
using Domain.Items;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.Items.Queries.FindMinMaxPrice;

public record FindMinMaxPriceQueryResponse(decimal MinPrice, decimal MaxPrice);

public record FindMinMaxPriceQuery : IRequest<FindMinMaxPriceQueryResponse>;

public sealed class FindMinMaxPriceQueryHandler(IItemRepository itemRepository, ICachingService cachingService)
    : IRequestHandler<FindMinMaxPriceQuery, FindMinMaxPriceQueryResponse>,
        ICacheableCqrsService<FindMinMaxPriceQueryResponse>
{
    public async Task<Option<FindMinMaxPriceQueryResponse>> RetrieveDataFromCache(
        string key,
        CancellationToken cancellationToken = default)
    {
        return await cachingService.RetrieveDataFromCache<FindMinMaxPriceQueryResponse>(key, cancellationToken);
    }

    public async Task StoreDataInCache(
        string key,
        FindMinMaxPriceQueryResponse data,
        CancellationToken cancellationToken = default)
    {
        TimeSpan expiration = TimeSpan.FromMinutes(5);

        DistributedCacheEntryOptions cacheEntryOptions = new CacheOptionsBuilder()
            .WithKey(key)
            .WithAbsoluteExpirationRelativeToNow(expiration)
            .Build();

        await cachingService.StoreDataInCache(key, data, cacheEntryOptions, cancellationToken);
    }

    public async Task<FindMinMaxPriceQueryResponse> Handle(FindMinMaxPriceQuery request,
        CancellationToken cancellationToken)
    {
        const string key = "min-max-price";

        return await cachingService.RetrieveDataFromCache<FindMinMaxPriceQueryResponse>(key, cancellationToken)
            .MatchAsync(
                response => response,
                async () =>
                {
                    FindMinMaxPriceQueryResponse findMinMaxPriceQueryResponse = await FindMinMaxValues(cancellationToken);
                    await StoreDataInCache(key, findMinMaxPriceQueryResponse, cancellationToken);

                    return findMinMaxPriceQueryResponse;
                }
            );
    }

    private async Task<FindMinMaxPriceQueryResponse> FindMinMaxValues(CancellationToken cancellationToken)
    {
        Specification<Item> itemSpecification = Specification<Item>.Create().And(i => i.IsActive);
        List<Item> listAllItemsBySpecificationAsync = await itemRepository.ListAllItemsBySpecificationAsync(itemSpecification, cancellationToken);

        decimal minPrice = int.MaxValue;
        decimal maxPrice = int.MinValue;

        foreach (Item item in listAllItemsBySpecificationAsync)
        {
            if (item.InitialPrice < minPrice) minPrice = item.InitialPrice;

            if (item.Bids.Count > 0)
            {
                decimal maxBid = item.Bids.Max(b => b.Amount);
                if (maxBid > maxPrice) maxPrice = maxBid;
            }
            else
            {
                if (item.InitialPrice > maxPrice) maxPrice = item.InitialPrice;
            }
        }

        FindMinMaxPriceQueryResponse findMinMaxPriceQueryResponse = new(minPrice, maxPrice);
        return findMinMaxPriceQueryResponse;
    }
}