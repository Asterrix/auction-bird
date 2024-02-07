using Application.Features.Items.Mapper;
using Application.Pagination;

namespace Application.Features.Items;

public interface IItemRepository
{
    Task<Page<ItemSummary>> ListAllAsync(Pageable pageable, CancellationToken cancellationToken = default);
}