namespace HotelBookingSystem.Application.DTOsNew.RoomTypeNew;

public class CreateRoomTypeRequestDto
{
    public Guid PropertyId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
}