namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class CreateReservationItemRequestDto
{
    public Guid? RoomId { get; set; }
    public Guid? BedId { get; set; }
    public decimal UnitPrice { get; set; }
}