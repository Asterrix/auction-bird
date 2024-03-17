using Domain.Items;
using LanguageExt;
using MediatR;

namespace Application.Features.Items.Queries.FindItemFullInfo;

public record FindItemFullInfoQuery(string ItemId) : IRequest<Option<Item>>;

public sealed class FindItemFullInfoQueryHandler(IItemRepository itemRepository) : IRequestHandler<FindItemFullInfoQuery, Option<Item>>
{
    public async Task<Option<Item>> Handle(FindItemFullInfoQuery request, CancellationToken cancellationToken)
    {
        Option<Item> item = await itemRepository.FindByIdAsync(Guid.Parse(request.ItemId), cancellationToken);
        return item;
    }
}