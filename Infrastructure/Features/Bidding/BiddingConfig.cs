using Domain.Bidding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Bidding;

public sealed class BiddingConfig : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.BidderId)
            .IsRequired();

        builder
            .Property(x => x.Amount)
            .IsRequired();

        builder
            .Property(x => x.PlacedAt)
            .IsRequired();
    }
}