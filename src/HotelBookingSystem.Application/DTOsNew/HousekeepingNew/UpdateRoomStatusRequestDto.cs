using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

public class UpdateRoomStatusRequestDto
{
    public Guid RoomId { get; set; }
    public RoomStatus Status { get; set; }
}