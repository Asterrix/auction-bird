using Domain.Categories;

namespace Application.Features.Categories.Queries;

public interface ICategoryRepository
{
    Task<List<Category>> ListAllAsync(CancellationToken cancellationToken = default);
}