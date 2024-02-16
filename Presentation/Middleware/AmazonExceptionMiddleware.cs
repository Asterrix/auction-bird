using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Middleware;

public sealed class AmazonExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AmazonServiceException amazonServiceException)
        {
            ProblemDetails problemDetails = new()
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = amazonServiceException.Message,
            };
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}