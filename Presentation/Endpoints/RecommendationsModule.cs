using Application.Features.Items.Mapper;
using Application.Features.Recommendations.Query.CreateRecommendationsRegisteredUser;
using Application.Features.Recommendations.Query.CreateRecommendationsRegularUser;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public sealed class RecommendationsModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/recommendations/regular", CreateRecommendationsRegularUser);
        app.MapGet("/recommendations/registered", CreateRecommendationsRegisteredUser);
    }

    private static async Task<IResult> CreateRecommendationsRegularUser(ISender sender, [FromQuery] int count)
    {
        List<ItemSummary> items = await sender.Send(new CreateRecommendationsRegularUserQuery(count));
        return Results.Ok(items);
    }
    
    private static async Task<IResult> CreateRecommendationsRegisteredUser(ISender sender, [FromQuery] string username, [FromQuery] int count)
    {
        List<ItemSummary> items = await sender.Send(new CreateRecommendationsRegisteredUserQuery(username, count));
        return Results.Ok(items);
    }
}