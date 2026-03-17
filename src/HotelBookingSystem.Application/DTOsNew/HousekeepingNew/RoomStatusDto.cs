using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

public class RoomStatusDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public RoomStatus Status { get; set; }
}