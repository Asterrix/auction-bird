namespace Domain.Bidding;

public sealed class Bid
{
    public int Id { get; }
    public string BidderId { get; init; }
    public Guid ItemId { get; init; }
    public decimal Amount { get; init; }
    public DateTime PlacedAt { get; init; }
}