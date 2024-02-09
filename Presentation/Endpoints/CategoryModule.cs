using Application.Features.Categories.Mapper;
using Application.Features.Categories.Queries.ListCategories;
using Carter;
using MediatR;

namespace Presentation.Endpoints;

public sealed class CategoryModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("categories", ListCategories);
    }

    private static async Task<IResult> ListCategories(ISender sender)
    {
        List<ParentCategory> categories = await sender.Send(new ListCategoriesQuery());
        return Results.Ok(categories);
    }
}