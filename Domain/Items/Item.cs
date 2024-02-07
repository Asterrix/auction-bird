using Domain.Categories;

namespace Domain.Items;

public sealed class Item
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal InitialPrice { get; init; }
    public Category Category { get; set; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool IsActive { get; set; } = true;
    public List<ItemImage> Images { get; set; } = [];
}