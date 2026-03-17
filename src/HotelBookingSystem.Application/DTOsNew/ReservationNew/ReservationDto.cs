using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class ReservationDto
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = default!;
    public Guid PropertyId { get; set; }
    public Guid GuestId { get; set; }
    public string GuestFullName { get; set; } = default!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ReservationStatus Status { get; set; }
    public ReservationSource Source { get; set; }
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public string? Notes { get; set; }
    public List<ReservationItemDto> Items { get; set; } = new();
}