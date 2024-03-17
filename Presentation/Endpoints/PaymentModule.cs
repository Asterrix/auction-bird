using Amazon.CognitoIdentityProvider.Model;
using Application.Features.Authentication.Queries;
using Application.Features.Items.Queries.FindItemFullInfo;
using Application.Features.Payment;
using Application.Features.Payment.Commands.CreateProduct;
using Application.Features.Payment.Commands.CreateSession;
using Carter;
using Domain.Bidding;
using Domain.Exceptions;
using Domain.Items;
using LanguageExt;
using MediatR;
using Stripe;
using Stripe.Checkout;

namespace Presentation.Endpoints;

public sealed class PaymentModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("payments", CreateSession);
    }

    private static async Task<IResult> CreateSession(ISender sender, HttpContext httpContext)
    {
        CreatePaymentSession createPaymentSession = httpContext.Request.Form.ToCreatePaymentSession();
        Option<UserType> userOption = await sender.Send(new FindUserQuery(createPaymentSession.Username));
        UserType user = userOption.IfNone(() => throw new NotFoundException("User not found."));

        Option<Item> itemOption = await sender.Send(new FindItemFullInfoQuery(createPaymentSession.ItemId));
        Item item = itemOption.IfNone(() => throw new NotFoundException("Item not found."));

        if (!item.IsActive) return Results.BadRequest("Item is no longer available.");
        if (item.Bids.Count == 0) return Results.BadRequest("Item has no bids.");

        Bid highestBid = item.Bids.MaxBy(x => x.Amount);
        if (highestBid.BidderId != createPaymentSession.Username)
            return Results.BadRequest("Requested user is not the highest bidder.");

        Option<string> customerIdOption = user.Attributes.Find(a => a.Name == "custom:customer_id").Value;
        string customerId = customerIdOption.IfNone(() => throw new InvalidStateException("Customer id is not present."));

        Product product = await sender.Send(new CreateProductCommand(item));
        Session session = await sender.Send(new CreateSessionCommand(customerId, product.DefaultPriceId));

        return Results.Ok(session);
    }
}