using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.DTOsNew.AvailabilityNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPublicCatalogService
{
    Task<PagedResultDto<PublicPropertyCardDto>> GetPropertiesAsync(
        PublicPropertyFilterRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PublicPropertyDetailsDto?> GetPropertyByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<List<AvailableRoomDto>> GetAvailableRoomsAsync(
        PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<List<AvailableBedDto>> GetAvailableBedsAsync(
        PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken = default);
}