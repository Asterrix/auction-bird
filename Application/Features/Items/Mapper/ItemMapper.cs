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
    decimal InitialPrice,
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
    
    public static ItemInfo ToInfo(this Item item, string timeRemaining)
    {
        return new ItemInfo(
            item.Id,
            item.Name,
            item.Description,
            item.InitialPrice,
            timeRemaining,
            item.IsActive,
            item.Images);
    }
}