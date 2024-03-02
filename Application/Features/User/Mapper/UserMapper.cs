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

public record SoldUserItemDto(
    Guid Id,
    string Name,
    decimal InitialPrice,
    int NumberOfBids,
    decimal FinalPrice,
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
    
    public static SoldUserItemDto MapToSoldUserItemDto(Item item, decimal finalPrice)
    {
        return new SoldUserItemDto(
            item.Id,
            item.Name,
            item.InitialPrice,
            item.Bids.Count,
            finalPrice,
            item.Images.First()
        );
    }
}