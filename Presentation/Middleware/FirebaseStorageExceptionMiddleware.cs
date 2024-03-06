using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Middleware;

public sealed class FirebaseStorageExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FirebaseStorageException e)
        {
            ProblemDetails problemDetails = new()
            {
                Title = "An error occurred while interacting with Firebase Storage.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = e.Message
            };
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}