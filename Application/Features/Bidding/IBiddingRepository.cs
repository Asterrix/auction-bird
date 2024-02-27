using Domain.Bidding;
using LanguageExt;

namespace Application.Features.Bidding;

public interface IBiddingRepository
{
    Task<bool> MakeBid(Bid bid);
}