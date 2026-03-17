using HotelBookingSystem.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.PersistenceNew.ConfigurationsNew;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable("ReservationItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.TotalPrice)
            .HasColumnType("numeric(18,2)");

        builder.HasOne(x => x.Reservation)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Bed)
            .WithMany()
            .HasForeignKey(x => x.BedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}