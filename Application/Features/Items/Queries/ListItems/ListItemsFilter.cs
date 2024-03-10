using Application.Filtration;
using Application.Specification;
using Domain.Items;
using LanguageExt.UnsafeValueAccess;

namespace Application.Features.Items.Queries.ListItems;

public class ListItemsFilter : IFilter<ListItemsQuery, ISpecification<Item>>
{
    // Be careful with the order of the filters
    public ISpecification<Item> Filter(ListItemsQuery request, CancellationToken cancellationToken)
    {
        ItemFilter filter = new();

        if (request.Categories is not null)
        {
            List<string> categories = request.Categories.Split(',').ToList();
            int n = categories.Count;

            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    filter.WithCategory(categories[i]);
                }
                else
                {
                    filter.WithOrCategory(categories[i]);
                }
            }
        }

        if (request.Search is not null)
        {
            filter.WithName(request.Search);
        }

        if (request.MinPrice.IsSome && request.MaxPrice.IsSome)
        {
            decimal minPrice = request.MinPrice.Value();
            decimal maxPrice = request.MaxPrice.Value();

            if (minPrice > maxPrice)
            {
                throw new ArgumentException("Min price cannot be greater than max price.");
            }

            if (minPrice < 0 || maxPrice < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
            
            filter.WithPriceRange(minPrice, maxPrice);
        }

        return filter;
    }
}