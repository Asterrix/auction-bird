using Amazon.CognitoIdentityProvider.Model;
using Application.Features.Authentication.Queries;
using Application.Features.Items;
using Application.Features.Items.Mapper;
using Application.Features.Recommendations.Query.CreateRecommendationsRegularUser;
using Application.Features.User.Queries.ListUserBiddingHistoryQuery;
using Application.Specification;
using Domain.Exceptions;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.Recommendations.Query.CreateRecommendationsRegisteredUser;

public record CreateRecommendationsRegisteredUserQuery(string UserId, int Count) : IRequest<List<ItemSummary>>;

public sealed class CreateRecommendationsRegisteredUserHandler(ISender sender, IItemRepository itemRepository)
    : IRequestHandler<CreateRecommendationsRegisteredUserQuery, List<ItemSummary>>
{
    private static readonly DateTime PriorTime = DateTime.UtcNow.AddMonths(-3);

    public async Task<List<ItemSummary>> Handle(CreateRecommendationsRegisteredUserQuery request,
        CancellationToken cancellationToken)
    {
        List<Item> userItems = await FetchUserHistory(request.UserId, PriorTime, cancellationToken);
        if (userItems.Count == 0)
        {
            return await CreateRegularUserRecommendations(request, cancellationToken);
        }

        string mostCommonCategory = FindMostCommonCategory(ref userItems);

        Specification<Item> specification = Specification<Item>.Create()
            .And(x => x.Category.Name == mostCommonCategory)
            .OrderBy(i => i.Bids.Count);

        List<Item> recommendedItems = await itemRepository.ListAllItemsBySpecificationAsync(specification, cancellationToken);
        if (recommendedItems.Count < request.Count)
        {
            throw new NotFoundException("Not enough items to create a recommendation for registered user.");
        }

        return recommendedItems.Take(request.Count).Select(x => x.ToSummary()).ToList();
    }

    private async Task<List<ItemSummary>> CreateRegularUserRecommendations(
        CreateRecommendationsRegisteredUserQuery request,
        CancellationToken cancellationToken)
    {
        List<ItemSummary> itemSummaries = await sender.Send(new CreateRecommendationsRegularUserQuery(request.Count), cancellationToken);
        if (itemSummaries.Count < request.Count)
        {
            throw new NotFoundException("Not enough items to create a recommendation for registered user.");
        }

        return itemSummaries;
    }

    private async Task<List<Item>> FetchUserHistory(string username, DateTime priorTime, CancellationToken cancellationToken)
    {
        Option<UserType> userOption = await sender.Send(new FindUserQuery(username), cancellationToken);
        UserType user = userOption.IfNone(() => throw new NotFoundException("User not found."));

        List<Item> userItems = await sender.Send(new ListUserBiddingHistoryQuery(user.Username, priorTime), cancellationToken);
        return userItems;
    }

    private static string FindMostCommonCategory(ref List<Item> items)
    {
        return items
            .GroupBy(x => x.Category)
            .OrderByDescending(x => x.Count())
            .First().Key.Name;
    }
}