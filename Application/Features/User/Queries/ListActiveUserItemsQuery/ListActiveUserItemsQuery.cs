using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Queries;
using Application.Features.Items;
using Application.Features.Items.Queries.FindItem;
using Application.Features.User.Mapper;
using Application.Pagination;
using Application.Specification;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.User.Queries.ListActiveUserItemsQuery;

public record ListActiveUserItemsQuery(string Username, Pageable Pageable) : IRequest<Page<ActiveUserItemDto>>;

public sealed class ListActiveUserItemsQueryHandler(
    ISender sender,
    IItemRepository itemRepository,
    ITimeRemainingCalculator timeRemainingCalculator)
    : IRequestHandler<ListActiveUserItemsQuery, Page<ActiveUserItemDto>>
{
    async Task<Page<ActiveUserItemDto>> IRequestHandler<ListActiveUserItemsQuery, Page<ActiveUserItemDto>>.Handle(
        ListActiveUserItemsQuery request,
        CancellationToken cancellationToken)
    {
        Option<UserType> userOption = await sender.Send(new FindUserQuery(request.Username), cancellationToken);
        UserType user = userOption.IfNone(() => throw new AmazonServiceException("User not found."));

        ISpecification<Item> specification = new ItemFilter()
            .IsActive()
            .OwnedBy(user.Username);

        Page<Item> items = await itemRepository.ListAllItemsFullDetailsAsync(request.Pageable, specification, cancellationToken);
        IEnumerable<ActiveUserItemDto> mappedItems = items.Elements.Select(item =>
        {
            string timeLeft = timeRemainingCalculator.CalculateTimeRemaining(item.EndTime);
            decimal highestBid = item.Bids.Count != 0 ? item.Bids.Max(bid => bid.Amount) : item.InitialPrice;
            
            return UserMapper.MapToActiveUserItemDto(item, timeLeft, highestBid);
        });
        
        return new Page<ActiveUserItemDto>(
            ref mappedItems,
            items.TotalElements,
            items.TotalPages,
            items.IsEmpty,
            items.IsLastPage);
    }
}