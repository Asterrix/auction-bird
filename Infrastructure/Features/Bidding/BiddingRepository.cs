using Application.Features.Bidding;
using Domain.Bidding;
using Infrastructure.Persistence;

namespace Infrastructure.Features.Bidding;

public sealed class BiddingRepository(DatabaseContext context) : IBiddingRepository
{
    public async Task<bool> MakeBid(Bid bid)
    {
        await context.Bids.AddAsync(bid);
        return await context.SaveChangesAsync() > 0;
    }
}