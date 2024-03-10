using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Features.Items.Commands.CreateItem;
using Application.Features.Items.Mapper;
using Application.Features.Items.Queries.FindItem;
using Application.Features.Items.Queries.FindMinMaxPrice;
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
        app.MapPost("items", CreateItem);
        app.MapGet("items/min-max-price", FindMinMaxPrice);
    }

    private static async Task<IResult> ListItems(
        ISender sender,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string? search = null,
        [FromQuery] string? categories = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        Pageable pageable = Pageable.Of(page, size);
        Option<decimal> minPriceOption = minPrice.Match(Option<decimal>.Some, () => Option<decimal>.None);
        Option<decimal> maxPriceOption = maxPrice.Match(Option<decimal>.Some, () => Option<decimal>.None);
        
        Page<ItemSummary> items = await sender.Send(new ListItemsQuery(pageable, search, categories, minPriceOption, maxPriceOption));

        return Results.Ok(items);
    }

    private static async Task<IResult> FindItem(
        ISender sender,
        Guid id)
    {
        Option<FindItemQueryResponse> item = await sender.Send(new FindItemQuery(id));

        return item.Match(
            i =>
            {
                ItemInfo mappedItems = ItemMapper.ToInfo(i);
                return Results.Ok(mappedItems);
            },
            Results.NotFound("Item not found.")
        );
    }
    
    private static async Task<IResult> CreateItem(ISender sender, HttpContext context, HttpRequest request)
    {
        string idToken = context.Request.Cookies["idToken"];
        if (string.IsNullOrEmpty(idToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Results.Unauthorized();
        }
        
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? token = handler.ReadJwtToken(idToken);

        string userId = token.Claims.First(claim => claim.Type == "sub").Value;
        if (string.IsNullOrEmpty(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Results.Unauthorized();
        }
        
        IFormCollection form = await request.ReadFormAsync();
        CreateItemDto createItemDto = form.ToCreateItemDto(userId);
        
        await sender.Send(new CreateItemCommand(createItemDto));

        return Results.Created($"/items/{0}", 0);
    }

    private static async Task<IResult> FindMinMaxPrice(ISender sender)
    {
        FindMinMaxPriceQueryResponse findMinMaxPriceQueryResponse = await sender.Send(new FindMinMaxPriceQuery());
        return Results.Ok(findMinMaxPriceQueryResponse);
    }
}