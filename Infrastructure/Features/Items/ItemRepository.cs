using System.Linq.Expressions;
using Application.Features.Items;
using Application.Features.Items.Mapper;
using Application.Pagination;
using Application.Specification;
using Domain.Items;
using Infrastructure.Persistence;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Items;

public sealed class ItemRepository(DatabaseContext context) : IItemRepository
{
    public async Task<Page<ItemSummary>> ListAllAsync(
        Pageable pageable,
        ISpecification<Item> specification,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Item, bool>> expression = specification.AsExpression();
        
        IEnumerable<ItemSummary> itemSummaries = await context.Items
            .OrderBy(i => i.Name)
            .Where(expression)
            .Select(item => new ItemSummary(item.Id, item.Name, item.InitialPrice, item.Images.First()))
            .Skip(pageable.Skip)
            .Take(pageable.Size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int totalItems = await context.Items.Where(expression).CountAsync(cancellationToken);

        return new Page<ItemSummary>(ref itemSummaries, pageable, totalItems);
    }

    public async Task<Option<Item>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Option<Item> item = await context.Items
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Bids)
            .Where(i => i.Id == id)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return item;
    }
}