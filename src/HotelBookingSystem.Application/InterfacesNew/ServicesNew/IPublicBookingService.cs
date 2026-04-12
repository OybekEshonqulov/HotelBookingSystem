using HotelBookingSystem.Application.DTOsNew.PublicNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPublicBookingService
{
    Task<PublicReservationResultDto> CreateReservationAsync(
        PublicCreateReservationRequestDto request,
        CancellationToken cancellationToken = default);
}