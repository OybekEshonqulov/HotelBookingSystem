namespace HotelBookingSystem.Application.DTOsNew.AvailabilityNew;

public class AvailableRoomDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
}