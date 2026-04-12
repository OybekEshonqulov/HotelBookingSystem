using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PaymentNew;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ReservationId { get; set; }
    public string ReservationNumber { get; set; } = default!;
    public string GuestFullName { get; set; } = default!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? TransactionId { get; set; }
    public DateTime PaidAtUtc { get; set; }
    public string? Notes { get; set; }
}