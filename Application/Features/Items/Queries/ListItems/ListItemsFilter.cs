using Application.Filtration;
using Application.Specification;
using Domain.Items;

namespace Application.Features.Items.Queries.ListItems;

public class ListItemsFilter : IFilter<ListItemsQuery, ISpecification<Item>>
{
    public ISpecification<Item> Filter(ListItemsQuery request, CancellationToken cancellationToken)
    {
        ItemFilter filter = new();

        if (request.Search is not null)
        {
            filter.WithName(request.Search);
        }

        return filter;
    }
}