using Amazon.CognitoIdentityProvider.Model;
using Application.Features.Authentication.Queries;
using Application.Features.Bidding.Mapper;
using Application.Features.Items.Queries.FindItem;
using Domain.Bidding;
using Domain.Items;
using FluentValidation;
using LanguageExt;
using MediatR;

namespace Application.Features.Bidding.Commands;

public record MakeBidCommand(MakeBidDto Bid) : IRequest<bool>;

public sealed class MakeBidCommandHandler : IRequestHandler<MakeBidCommand, bool>
{
    private readonly IBiddingRepository _biddingRepository;
    private readonly ISender _sender;

    public MakeBidCommandHandler(IBiddingRepository biddingRepository, ISender sender)
    {
        _biddingRepository = biddingRepository;
        _sender = sender;
    }

    public async Task<bool> Handle(MakeBidCommand request, CancellationToken cancellationToken)
    {
        Option<Item> itemOption = await FindItem(request, cancellationToken);
        Option<UserType> bidderOption = await FindBidder(request.Bid.UserId, cancellationToken);

        Item item = itemOption.IfNone(() => throw new ValidationException("Item not found."));
        UserType bidder = bidderOption.IfNone(() => throw new ValidationException("Bidder not found."));

        if (item.Bids.Count == 0)
        {
            return await MakeInitialBid(bidder, item, request.Bid.BidAmount);
        }

        return await MakeNewBid(bidder, item, request.Bid.BidAmount);
    }

    private Task<bool> MakeInitialBid(UserType bidder, Item item, decimal bidAmount)
    {
        if (bidAmount < item.InitialPrice)
        {
            throw new ValidationException("Bid amount must be higher than the starting price.");
        }

        Bid newBid = new()
        {
            BidderId = bidder.Username,
            ItemId = item.Id,
            Amount = bidAmount,
            PlacedAt = DateTime.UtcNow
        };

        return _biddingRepository.MakeBid(newBid);
    }

    private Task<bool> MakeNewBid(UserType bidder, Item item, decimal bidAmount)
    {
        decimal highestBidAmount = item.Bids.Max(b => b.Amount);

        if (highestBidAmount >= bidAmount)
        {
            throw new ValidationException("Bid amount must be higher than the current highest bid.");
        }

        Bid newBid = new()
        {
            BidderId = bidder.Username,
            ItemId = item.Id,
            Amount = bidAmount,
            PlacedAt = DateTime.UtcNow
        };

        return _biddingRepository.MakeBid(newBid);
    }

    private async Task<Option<UserType>> FindBidder(string username, CancellationToken cancellationToken)
    {
        Option<UserType> user = await _sender.Send(new FindUserQuery(username), cancellationToken);

        return user.Match(
            u => u,
            () => Option<UserType>.None
        );
    }

    private async Task<Option<Item>> FindItem(MakeBidCommand request, CancellationToken cancellationToken)
    {
        Option<FindItemQueryResponse> item = await _sender.Send(new FindItemQuery(request.Bid.ItemId), cancellationToken);

        return item.Match(
            i => i.Item,
            () => Option<Item>.None
        );
    }
}