using Domain.Categories;
using MediatR;

namespace Application.Features.Categories.Queries.ListCategories;

public record ListCategoriesQuery : IRequest<List<Category>>;

public class ListCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<ListCategoriesQuery, List<Category>>
{
    public async Task<List<Category>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await categoryRepository.ListAllAsync(cancellationToken);
    }
}