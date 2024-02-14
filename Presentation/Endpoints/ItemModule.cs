using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.FindItem;
using Application.Features.Items.Queries.ListItems;
using Application.Pagination;
using Carter;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public sealed class ItemModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("items", ListItems);
        app.MapGet("items/{id:guid}", FindItem);
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

    private static async Task<IResult> FindItem(
        ISender sender,
        Guid id)
    {
        Option<ItemInfo> item = await sender.Send(new FindItemQuery(id));

        return item.Match(
            i => Results.Ok(i),
            Results.NotFound("Item not found.")
        );
    }
}