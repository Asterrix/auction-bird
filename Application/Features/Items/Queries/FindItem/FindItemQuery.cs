using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.Items.Queries.FindItem;

public record FindItemQueryResponse(
    Item Item,
    string TimeTillStart,
    string TimeTillEnd,
    decimal CurrentPrice,
    bool AuctionStarted,
    bool AuctionFinished);

public record FindItemQuery(Guid Id) : IRequest<Option<FindItemQueryResponse>>;

public sealed class FindItemQueryHandler : IRequestHandler<FindItemQuery, Option<FindItemQueryResponse>>
{
    private readonly IItemRepository _itemRepository;
    private readonly ITimeRemainingCalculator _timeRemainingCalculator;

    public FindItemQueryHandler(IItemRepository itemRepository, ITimeRemainingCalculator timeRemainingCalculator)
    {
        _itemRepository = itemRepository;
        _timeRemainingCalculator = timeRemainingCalculator;
    }

    async Task<Option<FindItemQueryResponse>> IRequestHandler<FindItemQuery, Option<FindItemQueryResponse>>.Handle(FindItemQuery request, CancellationToken cancellationToken)
    {
        Option<Item> itemOption = await _itemRepository.FindByIdAsync(request.Id, cancellationToken);

        return itemOption.Match(
            item =>
            {
                bool auctionStarted = item.StartTime < DateTime.UtcNow;
                bool auctionFinished = item.EndTime < DateTime.UtcNow;
                string timeTillStart = _timeRemainingCalculator.CalculateTimeRemaining(item.StartTime);
                string timeTillEnd = _timeRemainingCalculator.CalculateTimeRemaining(item.EndTime);
                decimal currentPrice = item.Bids.Count != 0 ? item.Bids.Max(b => b.Amount) : item.InitialPrice;
                return new FindItemQueryResponse(item, timeTillStart, timeTillEnd, currentPrice, auctionStarted, auctionFinished);
            },
            () => Option<FindItemQueryResponse>.None);
    }
}