namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicRoomTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
    public int RoomsCount { get; set; }
    public int BedsCount { get; set; }
}