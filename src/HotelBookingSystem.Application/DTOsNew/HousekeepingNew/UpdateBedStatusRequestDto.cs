using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

public class UpdateBedStatusRequestDto
{
    public Guid BedId { get; set; }
    public BedStatus Status { get; set; }
}