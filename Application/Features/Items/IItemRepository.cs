using Application.Features.Items.Mapper;

namespace Application.Features.Items;

public interface IItemRepository
{
    Task<List<ItemSummary>> ListAllAsync(CancellationToken cancellationToken = default);
}