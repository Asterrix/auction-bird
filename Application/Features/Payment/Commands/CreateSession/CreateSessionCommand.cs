using MediatR;
using Stripe.Checkout;

namespace Application.Features.Payment.Commands.CreateSession;

public record CreateSessionCommand(string CustomerId, string Price) : IRequest<Session>;

public sealed class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Session>
{
    private readonly SessionService _sessionService = new();

    public async Task<Session> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        SessionCreateOptions options = new()
        {
            Currency = "usd",
            Customer = request.CustomerId,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    AdjustableQuantity = new SessionLineItemAdjustableQuantityOptions
                    {
                        Enabled = false
                    },
                    Quantity = 1,
                    Price = request.Price
                }
            ],
            Mode = "payment",
            SuccessUrl = "http://localhost:3000/payment/success"
        };

        Session session = await _sessionService.CreateAsync(options, cancellationToken: cancellationToken);

        return session;
    }
}