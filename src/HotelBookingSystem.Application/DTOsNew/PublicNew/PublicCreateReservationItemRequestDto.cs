namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicCreateReservationItemRequestDto
{
    public Guid? RoomId { get; set; }
    public Guid? BedId { get; set; }
}