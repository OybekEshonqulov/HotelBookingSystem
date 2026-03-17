namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class ReservationItemDto
{
    public Guid Id { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? BedId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Nights { get; set; }
    public decimal TotalPrice { get; set; }
}
