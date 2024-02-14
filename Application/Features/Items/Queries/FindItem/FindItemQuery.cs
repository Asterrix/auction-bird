using Application.Features.Items.Mapper;
using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.Items.Queries.FindItem;

public record FindItemQuery(Guid Id) : IRequest<Option<ItemInfo>>;

public sealed class FindItemQueryHandler : IRequestHandler<FindItemQuery, Option<ItemInfo>>
{
    private readonly IItemRepository _itemRepository;
    private readonly ITimeRemainingCalculator _timeRemainingCalculator;

    public FindItemQueryHandler(IItemRepository itemRepository, ITimeRemainingCalculator timeRemainingCalculator)
    {
        _itemRepository = itemRepository;
        _timeRemainingCalculator = timeRemainingCalculator;
    }

    public async Task<Option<ItemInfo>> Handle(FindItemQuery request, CancellationToken cancellationToken)
    {
        Option<Item> item = await _itemRepository.FindByIdAsync(request.Id, cancellationToken);
        
        return item.Match(
            Some: item =>
            {
                string timeRemaining = _timeRemainingCalculator.CalculateTimeRemaining(item.EndTime);
                return ItemMapper.ToInfo(item, timeRemaining);
            },
            None: Option<ItemInfo>.None
        );
    }
}