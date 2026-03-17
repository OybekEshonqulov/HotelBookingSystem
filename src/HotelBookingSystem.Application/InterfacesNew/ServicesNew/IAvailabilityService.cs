using HotelBookingSystem.Application.DTOsNew.AvailabilityNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IAvailabilityService
{
    Task<List<AvailableRoomDto>> GetAvailableRoomsAsync(AvailabilitySearchRequestDto request, CancellationToken cancellationToken = default);
    Task<List<AvailableBedDto>> GetAvailableBedsAsync(AvailabilitySearchRequestDto request, CancellationToken cancellationToken = default);
}