using LanguageExt;
using MediatR;
using Stripe;

namespace Application.Features.Payment.Queries;

public record FindCustomerQuery(string Id) : IRequest<Option<Customer>>;

public sealed class FindCustomerQueryHandler : IRequestHandler<FindCustomerQuery, Option<Customer>>
{
    private readonly CustomerService _customerService = new();
    
    public async Task<Option<Customer>> Handle(FindCustomerQuery request, CancellationToken cancellationToken)
    {
        Option<Customer> customer = await _customerService.GetAsync(request.Id, cancellationToken:cancellationToken);
        
        return customer;
    }
}