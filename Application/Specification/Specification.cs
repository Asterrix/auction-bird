using System.Linq.Expressions;

namespace Application.Specification;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> AsExpression();
}

public sealed class Specification<T>
{
    public Expression<Func<T, bool>> SpecificationExpression { get; private set; } = _ => true;

    private Specification()
    {
    }

    public static Specification<T> Create()
    {
        return new Specification<T>();
    }

    public Specification<T> And(Expression<Func<T, bool>> expression)
    {
        SpecificationExpression = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                SpecificationExpression.Body,
                Expression.Invoke(expression, SpecificationExpression.Parameters)
            ),
            SpecificationExpression.Parameters
        );
        return this;
    }

    public Specification<T> AndNot(Expression<Func<T, bool>> expression)
    {
        SpecificationExpression = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                SpecificationExpression.Body,
                Expression.Not(Expression.Invoke(expression, SpecificationExpression.Parameters))
            ),
            SpecificationExpression.Parameters
        );
        return this;
    }

    public Specification<T> Or(Expression<Func<T, bool>> expression)
    {
        SpecificationExpression = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                SpecificationExpression.Body,
                Expression.Invoke(expression, SpecificationExpression.Parameters)
            ),
            SpecificationExpression.Parameters
        );
        return this;
    }

    public Specification<T> OrNot(Expression<Func<T, bool>> expression)
    {
        SpecificationExpression = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                SpecificationExpression.Body,
                Expression.Not(Expression.Invoke(expression, SpecificationExpression.Parameters))
            ),
            SpecificationExpression.Parameters
        );
        return this;
    }

    public Specification<T> Not()
    {
        SpecificationExpression = Expression.Lambda<Func<T, bool>>(
            Expression.Not(SpecificationExpression.Body),
            SpecificationExpression.Parameters
        );
        return this;
    }
}