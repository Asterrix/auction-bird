using Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Items;

public sealed class ItemImagConfig : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(7000);

        const string shadowItemForeignKey = "ItemId";
        builder
            .Property<Guid>(shadowItemForeignKey)
            .IsRequired();
    }
}