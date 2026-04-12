using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class ReservationFilterRequestDto : PagedRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid? PropertyId { get; set; }
    public Guid? GuestId { get; set; }
    public ReservationStatus? Status { get; set; }
    public DateTime? CheckInFrom { get; set; }
    public DateTime? CheckInTo { get; set; }
    public string? Search { get; set; }
}