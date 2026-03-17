namespace HotelBookingSystem.Application.DTOsNew.CheckInOutNew;

public class CheckInRequestDto
{
    public Guid ReservationId { get; set; }
    public string? Notes { get; set; }
}