using Application.Features.Items;
using Application.Features.Items.Mapper;
using Application.Pagination;
using Application.Specification;
using Domain.Items;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Items;

public sealed class ItemRepository(DatabaseContext context) : IItemRepository
{
    public async Task<Page<ItemSummary>> ListAllAsync(
        Pageable pageable,
        ISpecification<Item> specification,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<ItemSummary> itemSummaries = await context.Items
            .OrderBy(i => i.Name)
            .Where(specification.AsExpression())
            .Select(item => new ItemSummary(item.Id, item.Name, item.InitialPrice, item.Images.First()))
            .Skip(pageable.Skip)
            .Take(pageable.Size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int totalItems = await context.Items.CountAsync(cancellationToken);

        return new Page<ItemSummary>(ref itemSummaries, pageable, totalItems);
    }
}