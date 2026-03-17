namespace HotelBookingSystem.Application.DTOsNew.AvailabilityNew;

public class AvailabilitySearchRequestDto
{
    public Guid PropertyId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}