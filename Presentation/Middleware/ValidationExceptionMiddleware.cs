using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Middleware;

public sealed class ValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            ProblemDetails problemDetails = new()
            {
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = e.Errors.First().ErrorMessage,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}