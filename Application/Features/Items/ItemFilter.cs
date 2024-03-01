using System.Linq.Expressions;
using Application.Specification;
using Domain.Items;

namespace Application.Features.Items;

public sealed class ItemFilter : ISpecification<Item>
{
    private readonly Specification<Item> _specification = Specification<Item>.Create();

    // Should come after Category filter
    public ItemFilter WithName(string name)
    {
        _specification.And(i => i.Name.ToLower().Contains(name.ToLower()));
        return this;
    }

    public ItemFilter WithCategory(string category)
    {
        _specification.And(c => c.Category.Name.ToLower() == category.ToLower());
        return this;
    }
    
    public ItemFilter WithOrCategory(string category)
    {
        _specification.Or(c => c.Category.Name.ToLower() == category.ToLower());
        return this;
    }
    
    public ItemFilter IsActive()
    {
        _specification.And(i => i.IsActive);
        return this;
    }
    
    public ItemFilter OwnedBy(string username)
    {
        _specification.And(i => i.OwnerId == username);
        return this;
    }
    
    public Expression<Func<Item, bool>> AsExpression()
    {
        return _specification.SpecificationExpression;
    }
}