using HotelBookingSystem.Application.DTOsNew.CommonNew;

namespace HotelBookingSystem.Application.DTOsNew.PaymentNew;

public class PaymentFilterRequestDto : PagedRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid? ReservationId { get; set; }
    public DateTime? PaidFrom { get; set; }
    public DateTime? PaidTo { get; set; }
    public string? Search { get; set; }
}