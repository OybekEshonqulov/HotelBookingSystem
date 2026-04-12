using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class ChangeReservationStatusRequestDto
{
    public ReservationStatus Status { get; set; }
    public string? Notes { get; set; }
}