using Microsoft.AspNetCore.Http;

namespace Application.Features.Payment;

public record CreatePaymentSession(
    string Username,
    string ItemId
);

public static class PaymentMapper
{
    public static CreatePaymentSession ToCreatePaymentSession(this IFormCollection form)
    {
        return new CreatePaymentSession(
            form["username"],
            form["itemId"]
        );
    }
}