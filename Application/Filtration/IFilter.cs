using Application.Specification;
using MediatR;

namespace Application.Filtration;

public interface IFilter<in TRequest, TResponse> : IRequest<ISpecification<TResponse>>
{
    TResponse Filter(TRequest request, CancellationToken cancellationToken);
}