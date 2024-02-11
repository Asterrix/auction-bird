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

        if (request.Categories is not null)
        {
            List<string> categories = request.Categories.Split(',').ToList();
            int n = categories.Count;

            for (int i = 0; i < n; i++)
            {
                if(i == 0) filter.WithCategory(categories[i]);
                else filter.WithOrCategory(categories[i]);
            }
        }

        return filter;
    }
}