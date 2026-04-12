using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicReservationResultDto
{
    public Guid ReservationId { get; set; }
    public string ReservationNumber { get; set; } = default!;
    public ReservationStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "UZS";
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string GuestFullName { get; set; } = default!;
}