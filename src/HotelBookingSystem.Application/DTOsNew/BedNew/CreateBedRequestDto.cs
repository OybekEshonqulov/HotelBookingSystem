namespace HotelBookingSystem.Application.DTOsNew.BedNew;

public class CreateBedRequestDto
{
    public Guid RoomId { get; set; }
    public string BedCode { get; set; } = default!;
    public decimal? BedPrice { get; set; }
}