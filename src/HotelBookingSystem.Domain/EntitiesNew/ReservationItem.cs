using HotelBookingSystem.Domain.CommonNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class ReservationItem : TenantEntity
{
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }

    public Guid? BedId { get; set; }
    public Bed? Bed { get; set; }

    public decimal UnitPrice { get; set; }
    public int Nights { get; set; }
    public decimal TotalPrice { get; set; }
}