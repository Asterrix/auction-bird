using Application.Features.Items.Mapper;
using MediatR;

namespace Application.Features.Items.Queries.ListItems;

public record ListItemsQuery : IRequest<List<ItemSummary>>;

public sealed class ListItemsQueryHandler(IItemRepository itemRepository) : IRequestHandler<ListItemsQuery, List<ItemSummary>>
{
    public async Task<List<ItemSummary>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        return await itemRepository.ListAllAsync(cancellationToken);
    }
}