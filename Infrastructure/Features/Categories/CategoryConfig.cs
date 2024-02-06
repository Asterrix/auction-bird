using Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Categories;

public sealed class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        const string shadowPropertyName = "ParentId";
        builder
            .Property<int?>(shadowPropertyName)
            .IsRequired(false);

        builder
            .HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(shadowPropertyName)
            .OnDelete(DeleteBehavior.Restrict);
    }
}