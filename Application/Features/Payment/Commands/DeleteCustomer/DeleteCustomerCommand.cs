using Application.Features.Payment.Queries;
using Domain.Exceptions;
using LanguageExt;
using MediatR;
using Stripe;

namespace Application.Features.Payment.Commands.DeleteCustomer;

public record DeleteCustomerCommand(string Email) : IRequest<bool>;

public sealed class DeleteCustomerCommandHandler(ISender sender) : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly CustomerService _customerService = new();

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        Option<Customer> customerOption = await sender.Send(new FindCustomerQuery(request.Email), cancellationToken);
        Customer customer = customerOption.IfNone(() => throw new NotFoundException("Customer could not be found."));

        Option<Customer> removedCustomer = await _customerService.DeleteAsync(customer.Email, cancellationToken: cancellationToken);

        return removedCustomer.IsNone;
    }
}