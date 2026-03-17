using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class ReservationFilterRequestDto : PagedRequestDto
{
    public Guid PropertyId { get; set; }
    public ReservationStatus? Status { get; set; }
    public string? GuestName { get; set; }
    public DateTime? CheckInFrom { get; set; }
    public DateTime? CheckInTo { get; set; }
}