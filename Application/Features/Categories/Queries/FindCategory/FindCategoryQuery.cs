using Domain.Categories;
using LanguageExt;
using MediatR;

namespace Application.Features.Categories.Queries.FindCategory;

public record FindCategoryQuery(string Name) : IRequest<Option<Category>>;

public sealed class FindCategoryQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<FindCategoryQuery, Option<Category>>
{
    public async Task<Option<Category>> Handle(FindCategoryQuery request, CancellationToken cancellationToken)
    {
        return await categoryRepository.FindByNameAsync(request.Name, cancellationToken);
    }
}