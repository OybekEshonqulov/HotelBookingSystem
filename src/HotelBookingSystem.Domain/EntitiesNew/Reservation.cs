using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class Reservation : TenantEntity
{
    public string ReservationNumber { get; set; } = default!;

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public Guid GuestId { get; set; }
    public Guest Guest { get; set; } = default!;

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public ReservationSource Source { get; set; } = ReservationSource.AdminPanel;

    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string CurrencyCode { get; set; } = "UZS";
    public string? Notes { get; set; }

    public ICollection<ReservationItem> Items { get; set; } = new List<ReservationItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}