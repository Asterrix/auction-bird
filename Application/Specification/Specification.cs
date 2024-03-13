using System.Linq.Expressions;
using LanguageExt;

namespace Application.Specification;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> AsExpression();
}

public sealed class Specification<T>
{
    public Expression<Func<T, bool>> SpecificationExpression { get; private set; } = _ => true;
    public Either<Expression<Func<T, bool>>, Expression<Func<T, int>>> OrderByExpression { get; private set; } = Either<Expression<Func<T, bool>>, Expression<Func<T, int>>>.Left(x => true);
    public int TakeExpression { get; private set; } = 0;

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
    
    public Specification<T> OrderBy(Expression<Func<T, bool>> expression)
    {
        OrderByExpression = Either<Expression<Func<T, bool>>, Expression<Func<T, int>>>.Left(expression);
        return this;
    }

    public Specification<T> OrderBy(Expression<Func<T, int>> expression)
    {
        OrderByExpression = Either<Expression<Func<T, bool>>, Expression<Func<T, int>>>.Right(expression);
        return this;
    }
    
    public Specification<T> Take(int count)
    {
        TakeExpression = count;
        return this;
    }
}