using Application.Features.Items;
using Application.Features.Items.Mapper;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Items;

public sealed class ItemRepository(DatabaseContext context) : IItemRepository
{
    public async Task<List<ItemSummary>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Items
            .OrderBy(i => i.Name)
            .Select(item => new ItemSummary(item.Id, item.Name, item.InitialPrice, item.Images.First()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}