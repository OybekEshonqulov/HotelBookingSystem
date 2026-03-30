using HotelBookingSystem.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.PersistenceNew.Configurations;

public class PropertyReviewConfiguration : IEntityTypeConfiguration<PropertyReview>
{
    public void Configure(EntityTypeBuilder<PropertyReview> builder)
    {
        builder.ToTable("PropertyReviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Property)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}