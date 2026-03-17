using HotelBookingSystem.Application.DTOsNew.HousekeepingNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IHousekeepingService
{
    Task<RoomStatusDto> UpdateRoomStatusAsync(UpdateRoomStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<BedStatusDto> UpdateBedStatusAsync(UpdateBedStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<List<RoomStatusDto>> GetRoomsByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<List<BedStatusDto>> GetBedsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}