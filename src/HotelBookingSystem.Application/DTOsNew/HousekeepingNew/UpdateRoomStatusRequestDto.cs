using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

public class UpdateRoomStatusRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid RoomId { get; set; }
    public RoomStatus Status { get; set; }
}