using Amazon.CognitoIdentityProvider.Model;
using Application.Features.Authentication.Queries;
using Application.Features.Items;
using Application.Specification;
using Domain.Exceptions;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.User.Queries.ListUserBiddingHistoryQuery;

public record ListUserBiddingHistoryQuery(string Username, DateTime PriorTime) : IRequest<List<Item>>;

public sealed class ListUserBiddingHistoryQueryHandler(
    ISender sender,
    IItemRepository itemRepository) : IRequestHandler<ListUserBiddingHistoryQuery, List<Item>>
{
    public async Task<List<Item>> Handle(ListUserBiddingHistoryQuery request, CancellationToken cancellationToken)
    {
        Option<UserType> userOption = await sender.Send(new FindUserQuery(request.Username), cancellationToken);
        UserType user = userOption.IfNone(() => throw new NotFoundException("User not found"));

        Specification<Item> specification = Specification<Item>.Create()
            .And(i => i.Bids.Any(b => b.BidderId == user.Username))
            .And(i => i.Bids.Any(b => b.PlacedAt >= request.PriorTime));

        List<Item> userItems = await itemRepository.ListAllItemsBySpecificationAsync(specification, cancellationToken);
        
        return userItems;
    }
}