namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicAvailabilitySearchRequestDto
{
    public Guid PropertyId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}