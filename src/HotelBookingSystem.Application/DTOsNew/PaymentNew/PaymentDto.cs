using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PaymentNew;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaidAtUtc { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
}