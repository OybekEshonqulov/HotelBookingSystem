namespace HotelBookingSystem.Application.DTOsNew.RoomNew;

public class CreateRoomRequestDto
{
    public Guid PropertyId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Number { get; set; } = default!;
    public int Floor { get; set; }
}