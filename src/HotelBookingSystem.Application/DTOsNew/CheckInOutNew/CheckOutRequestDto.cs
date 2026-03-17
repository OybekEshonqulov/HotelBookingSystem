namespace HotelBookingSystem.Application.DTOsNew.CheckInOutNew;

public class CheckOutRequestDto
{
    public Guid ReservationId { get; set; }
    public string? Notes { get; set; }
}