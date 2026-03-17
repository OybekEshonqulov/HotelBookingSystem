using HotelBookingSystem.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.PersistenceNew.ConfigurationsNew;

public class BedConfiguration : IEntityTypeConfiguration<Bed>
{
    public void Configure(EntityTypeBuilder<Bed> builder)
    {
        builder.ToTable("Beds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BedCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BedPrice)
            .HasColumnType("numeric(18,2)");

        builder.HasIndex(x => new { x.RoomId, x.BedCode })
            .IsUnique();
        builder.Property(x => x.Status).IsRequired();
        builder.HasOne(x => x.Room)
            .WithMany(x => x.Beds)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}