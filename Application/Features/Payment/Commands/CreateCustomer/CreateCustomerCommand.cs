using Domain.Exceptions;
using LanguageExt;
using MediatR;
using Stripe;

namespace Application.Features.Payment.Commands.CreateCustomer;

public record CreateCustomerCommand(string Email) : IRequest<Customer>;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Customer>
{
    private readonly CustomerService _customerService = new();

    public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        CustomerCreateOptions customerCreate = new()
        {
            Email = request.Email
        };

        Option<Customer> customerOption = await _customerService.CreateAsync(customerCreate, cancellationToken: cancellationToken);
        Customer customer = customerOption.IfNone(() => throw new InvalidStateException("Failed to create customer."));
        
        return customer;
    }
}