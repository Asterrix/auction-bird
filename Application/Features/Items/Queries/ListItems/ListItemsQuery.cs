using Application.Features.Items.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Items.Queries.ListItems;

public record ListItemsQuery(Pageable Pageable) : IRequest<Page<ItemSummary>>;

public sealed class ListItemsQueryHandler(IItemRepository itemRepository) : IRequestHandler<ListItemsQuery, Page<ItemSummary>>
{
    public async Task<Page<ItemSummary>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        return await itemRepository.ListAllAsync(request.Pageable, cancellationToken);
    }
}