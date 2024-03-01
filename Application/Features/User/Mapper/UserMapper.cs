using Domain.Items;

namespace Application.Features.User.Mapper;

public record ActiveUserItemDto(
    Guid Id,
    string Name,
    string TimeLeft,
    decimal InitialPrice,
    int NumberOfBids,
    decimal HighestBid,
    ItemImage MainImage
);

public static class UserMapper
{
    public static ActiveUserItemDto MapToActiveUserItemDto(Item item, string timeLeft, decimal highestBid)
    {
        return new ActiveUserItemDto(
            item.Id,
            item.Name,
            timeLeft,
            item.InitialPrice,
            item.Bids.Count,
            highestBid,
            item.Images.First()
        );
    }
}