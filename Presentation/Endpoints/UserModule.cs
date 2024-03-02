using Application.Features.User.Mapper;
using Application.Features.User.Queries.ListActiveUserItemsQuery;
using Application.Features.User.Queries.ListSoldUserItemsQuery;
using Application.Pagination;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public sealed class UserModule() : CarterModule(Versioning.Version)
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{username}/items", ListActiveUserItems);
        app.MapGet("users/{username}/sold-items", ListSoldUserItems);
    }

    private static async Task<IResult> ListActiveUserItems(
        ISender sender,
        string username,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        Pageable pageable = Pageable.Of(page, size);
        Page<ActiveUserItemDto> items = await sender.Send(new ListActiveUserItemsQuery(username, pageable));

        return Results.Ok(items);
    }
    
    private static async Task<IResult> ListSoldUserItems(
        ISender sender,
        string username,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        Pageable pageable = Pageable.Of(page, size);
        Page<SoldUserItemDto> items = await sender.Send(new ListSoldUserItemsQuery(username, pageable));

        return Results.Ok(items);
    }
}