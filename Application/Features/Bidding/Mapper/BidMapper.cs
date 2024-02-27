using Microsoft.AspNetCore.Http;

namespace Application.Features.Bidding.Mapper;

public record MakeBidDto(
    string UserId,
    Guid ItemId,
    decimal BidAmount
);

public static class BidMapper
{
    public static MakeBidDto MapToMakeBidDto(this IFormCollection form, string userId)
    {
        return new MakeBidDto(
            userId,
            Guid.Parse(form["ItemId"]),
            decimal.Parse(form["BidAmount"])
        );
    }
}