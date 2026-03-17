using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class Payment : TenantEntity
{
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
    public string CurrencyCode { get; set; } = "UZS";
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
}