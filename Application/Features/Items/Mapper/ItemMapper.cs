using Application.Features.Items.Queries.FindItem;
using Domain.Items;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Items.Mapper;

public record ItemSummary(
    Guid Id,
    string Name,
    decimal InitialPrice,
    ItemImage MainImage);

public record ItemInfo(
    Guid Id,
    string Name,
    string Description,
    decimal CurrentPrice,
    string TimeLeft,
    bool IsActive,
    List<ItemImage> Images);

public record CreateItemDto(
    string OwnerId,
    string Name,
    string Category,
    string Subcategory,
    string Description,
    decimal InitialPrice,
    DateTime StartTime,
    DateTime EndTime,
    List<IFormFile> Images
);

public static class ItemMapper
{
    public static ItemSummary ToSummary(this Item item)
    {
        return new ItemSummary(item.Id, item.Name, item.InitialPrice, item.Images.First());
    }

    public static IEnumerable<ItemSummary> ToSummary(this IEnumerable<Item> items)
    {
        return items.Select(ToSummary);
    }

    public static ItemInfo ToInfo(FindItemQueryResponse itemQueryResponse)
    {
        return new ItemInfo(
            itemQueryResponse.Item.Id,
            itemQueryResponse.Item.Name,
            itemQueryResponse.Item.Description,
            itemQueryResponse.CurrentPrice,
            itemQueryResponse.TimeRemaining,
            itemQueryResponse.Item.IsActive,
            itemQueryResponse.Item.Images);
    }
    
    public static CreateItemDto ToCreateItemDto(this IFormCollection form, string ownerId)
    {
        return new CreateItemDto(
            ownerId,
            form["Name"],
            form["Category"],
            form["Subcategory"],
            form["Description"],
            decimal.Parse(form["InitialPrice"]),
            DateTime.Parse(form["StartTime"]),
            DateTime.Parse(form["EndTime"]),
            form.Files.ToList());
    }
}