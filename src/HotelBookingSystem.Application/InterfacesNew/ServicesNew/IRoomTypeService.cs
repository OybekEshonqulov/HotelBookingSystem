using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomTypeNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IRoomTypeService
{
    Task<RoomTypeDto> CreateAsync(CreateRoomTypeRequestDto request, CancellationToken cancellationToken = default);
    Task<List<RoomTypeDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<RoomTypeDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default);
}