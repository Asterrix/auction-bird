using Application.Features.Items.Queries.FindItemFullInfo;
using Domain.Exceptions;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.Bidding.Queries.FindHighestBidder;

public record FindHighestBidderQuery(Guid ItemId) : IRequest<string>;

public sealed class FindHighestBidderQueryHandler(ISender sender) : IRequestHandler<FindHighestBidderQuery, string>
{
    public async Task<string> Handle(FindHighestBidderQuery request, CancellationToken cancellationToken)
    {
        Option<Item> itemOption = await sender.Send(new FindItemFullInfoQuery(request.ItemId.ToString()), cancellationToken);
        Item item = itemOption.IfNone(() => throw new NotFoundException("Item not found."));

        if (item.Bids.Count == 0) throw new InvalidStateException("Item has no bids.");
        
        return item.Bids.MaxBy(x => x.Amount).BidderId;
    }
}