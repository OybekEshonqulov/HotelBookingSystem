using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

public class BedStatusDto
{
    public Guid BedId { get; set; }
    public string BedCode { get; set; } = default!;
    public BedStatus Status { get; set; }
}