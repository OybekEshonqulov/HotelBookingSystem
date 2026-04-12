using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IRoomService
{
    Task<RoomDto> CreateAsync(CreateRoomRequestDto request, CancellationToken cancellationToken = default);
    Task<List<RoomDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<RoomDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default);
}