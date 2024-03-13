using Application.Features.Items;
using Application.Features.Items.Mapper;
using Application.Specification;
using Domain.Exceptions;
using Domain.Items;
using FluentValidation;
using MediatR;

namespace Application.Features.Recommendations.Query.CreateRecommendationsRegularUser;

public sealed class
    CreateRecommendationsRegularUserQueryValidator : AbstractValidator<CreateRecommendationsRegularUserQuery>
{
    public CreateRecommendationsRegularUserQueryValidator()
    {
        RuleFor(p => p.Count)
            .GreaterThan(0)
            .WithMessage("Item count must be greater than 0.")
            .LessThan(65)
            .WithMessage("Item count must be less than 64.");
    }
}

public record CreateRecommendationsRegularUserQuery(int Count) : IRequest<List<ItemSummary>>;

public sealed class CreateRecommendationsRegularUserQueryHandler(IItemRepository itemRepository)
    : IRequestHandler<CreateRecommendationsRegularUserQuery, List<ItemSummary>>
{
    private const int ItemsToShuffle = 64;

    public async Task<List<ItemSummary>> Handle(CreateRecommendationsRegularUserQuery request,
        CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        Specification<Item> specification = Specification<Item>.Create()
            .And(i => i.IsActive)
            .And(i => i.StartTime <= currentTime)
            .And(i => i.EndTime >= currentTime)
            .And(i => i.Bids.Count > 0)
            .OrderBy(i => i.Bids.Count)
            .Take(ItemsToShuffle);

        List<Item> items = await itemRepository.ListAllItemsBySpecificationAsync(specification, cancellationToken);
        if (items.Count < request.Count)
        {
            throw new NotFoundException("There are not enough items to create recommendations.");
        }

        Shuffle(items);

        return items
            .ToSummary()
            .Take(request.Count)
            .ToList();
    }

    private static void Shuffle<T>(IList<T> list)
    {
        Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}