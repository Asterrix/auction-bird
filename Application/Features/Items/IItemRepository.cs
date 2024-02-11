using Application.Features.Items.Mapper;
using Application.Pagination;
using Application.Specification;
using Domain.Items;

namespace Application.Features.Items;

public interface IItemRepository
{
    Task<Page<ItemSummary>> ListAllAsync(
        Pageable pageable,
        ISpecification<Item> specification,
        CancellationToken cancellationToken = default);
}