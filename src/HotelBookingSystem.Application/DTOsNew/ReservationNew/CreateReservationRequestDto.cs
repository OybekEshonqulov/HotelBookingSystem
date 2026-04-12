using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class CreateReservationRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ReservationSource Source { get; set; }
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "UZS";
    public string? Notes { get; set; }
}