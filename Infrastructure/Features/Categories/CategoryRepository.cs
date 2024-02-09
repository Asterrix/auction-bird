using Application.Features.Categories.Queries;
using Domain.Categories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Categories;

public sealed class CategoryRepository(DatabaseContext context) : ICategoryRepository
{
    public async Task<List<Category>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(x => x.Parent)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}