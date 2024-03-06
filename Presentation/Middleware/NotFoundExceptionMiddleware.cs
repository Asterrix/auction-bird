using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Middleware;

public class NotFoundExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException e)
        {
            ProblemDetails problemDetails = new()
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = e.Message
            };
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/problem+json";
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}