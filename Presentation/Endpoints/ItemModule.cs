using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.ListItems;
using Application.Pagination;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public sealed class ItemModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("items", ListItems);
    }

    private static async Task<IResult> ListItems(
        ISender sender,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string? search = null,
        [FromQuery] string? categories = null)
    {
        Pageable pageable = Pageable.Of(page, size);
        Page<ItemSummary> items = await sender.Send(new ListItemsQuery(pageable, search, categories));

        return Results.Ok(items);
    }
}