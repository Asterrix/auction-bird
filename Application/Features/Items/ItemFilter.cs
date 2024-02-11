using System.Linq.Expressions;
using Application.Specification;
using Domain.Items;

namespace Application.Features.Items;

public sealed class ItemFilter : ISpecification<Item>
{
    private readonly Specification<Item> _specification = Specification<Item>.Create();

    public ItemFilter WithName(string name)
    {
        _specification.And(i => i.Name.ToLower().Contains(name.ToLower()));
        return this;
    }


    public Expression<Func<Item, bool>> AsExpression()
    {
        return _specification.SpecificationExpression;
    }
}