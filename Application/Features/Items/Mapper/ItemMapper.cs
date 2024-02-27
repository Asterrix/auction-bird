using Application.Features.Items.Queries.FindItem;
using Domain.Items;

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
}