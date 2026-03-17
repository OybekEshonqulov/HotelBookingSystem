using HotelBookingSystem.Application.DTOsNew.RoomTypeNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IRoomTypeService
{
    Task<RoomTypeDto> CreateAsync(CreateRoomTypeRequestDto request, CancellationToken cancellationToken = default);
    Task<List<RoomTypeDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);
}