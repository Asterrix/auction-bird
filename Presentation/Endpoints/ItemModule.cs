using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.ListItems;
using Carter;
using MediatR;

namespace Presentation.Endpoints;

public sealed class ItemModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("items", ListItems);
    }

    private static async Task<IResult> ListItems(ISender sender)
    {
        List<ItemSummary> items = await sender.Send(new ListItemsQuery());

        return Results.Ok(items);
    }
}