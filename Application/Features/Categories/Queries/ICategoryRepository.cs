using Domain.Categories;
using LanguageExt;

namespace Application.Features.Categories.Queries;

public interface ICategoryRepository
{
    Task<List<Category>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<Option<Category>> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}