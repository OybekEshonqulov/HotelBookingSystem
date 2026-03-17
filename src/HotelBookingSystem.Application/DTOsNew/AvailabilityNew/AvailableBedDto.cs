namespace HotelBookingSystem.Application.DTOsNew.AvailabilityNew;

public class AvailableBedDto
{
    public Guid BedId { get; set; }
    public string BedCode { get; set; } = default!;
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = default!;
    public decimal? BedPrice { get; set; }
}