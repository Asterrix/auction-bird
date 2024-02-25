using Application.Features.Authentication.Commands.ValidateToken;
using LanguageExt;
using MediatR;

namespace Presentation.Middleware;

public sealed class TokenMiddleware(ISender sender, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? clientId = context.Request.Headers["ClientId"];
        string? idToken = context.Request.Cookies["idToken"];
        string? refreshToken = context.Request.Cookies["refreshToken"];

        if (clientId is not null && idToken is not null && refreshToken is not null)
        {
            bool isValid = await sender.Send(new ValidateTokenCommand(clientId, idToken, refreshToken));

            if (isValid)
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
        else if (clientId is not null && idToken is not null)
        {
            bool isValid = await sender.Send(new ValidateTokenCommand(clientId, idToken, Option<string>.None));

            if (isValid)
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
        else
        {
            await next(context);
        }
    }
}