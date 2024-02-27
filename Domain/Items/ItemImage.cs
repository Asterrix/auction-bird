namespace Domain.Items;

public sealed class ItemImage
{
    public int Id { get; init; }
    public string ImageUrl { get; set; }

    public Guid ItemId { get; set; }
}