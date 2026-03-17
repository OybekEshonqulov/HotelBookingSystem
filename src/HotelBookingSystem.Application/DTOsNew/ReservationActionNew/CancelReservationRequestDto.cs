namespace HotelBookingSystem.Application.DTOsNew.ReservationActionNew;

public class CancelReservationRequestDto
{
    public Guid ReservationId { get; set; }
    public string? Reason { get; set; }
}