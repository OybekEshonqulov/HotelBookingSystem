using HotelBookingSystem.Application.DTOsNew.BedNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IBedService
{
    Task<BedDto> CreateAsync(CreateBedRequestDto request, CancellationToken cancellationToken = default);
    Task<List<BedDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}