using Domain.Items;

namespace Application.Features.Items.Mapper;

public record ItemSummary(Guid Id, string Name, decimal InitialPrice, ItemImage MainImage);

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
}