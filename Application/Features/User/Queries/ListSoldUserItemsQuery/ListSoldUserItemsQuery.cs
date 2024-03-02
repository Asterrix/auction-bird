using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Queries;
using Application.Features.Items;
using Application.Features.User.Mapper;
using Application.Pagination;
using Application.Specification;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.User.Queries.ListSoldUserItemsQuery;

public record ListSoldUserItemsQuery(string Username, Pageable Pageable) : IRequest<Page<SoldUserItemDto>>;

public sealed class ListSoldUserItemsQueryHandler(
    ISender sender,
    IItemRepository itemRepository) : IRequestHandler<ListSoldUserItemsQuery, Page<SoldUserItemDto>>
{
    public async Task<Page<SoldUserItemDto>> Handle(ListSoldUserItemsQuery request, CancellationToken cancellationToken)
    {
        Option<UserType> userOption = await sender.Send(new FindUserQuery(request.Username), cancellationToken);
        UserType user = userOption.IfNone(() => throw new AmazonServiceException("User not found."));
        
        ISpecification<Item> specification = new ItemFilter()
            .IsSold()
            .OwnedBy(user.Username);
        
        Page<Item> items = await itemRepository.ListAllItemsFullDetailsAsync(request.Pageable, specification, cancellationToken);
        IEnumerable<SoldUserItemDto> mappedItems = items.Elements.Select(item =>
        {
            decimal finalPrice = item.Bids.Count != 0 ? item.Bids.Max(bid => bid.Amount) : 0;
            return UserMapper.MapToSoldUserItemDto(item, finalPrice);
        });
        
        return new Page<SoldUserItemDto>(
            ref mappedItems,
            items.TotalElements,
            items.TotalPages,
            items.IsEmpty,
            items.IsLastPage);
    }
}