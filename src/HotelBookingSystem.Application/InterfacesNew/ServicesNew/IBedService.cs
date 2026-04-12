using HotelBookingSystem.Application.DTOsNew.BedNew;
using HotelBookingSystem.Application.DTOsNew.CommonNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IBedService
{
    Task<BedDto> CreateAsync(CreateBedRequestDto request, CancellationToken cancellationToken = default);
    Task<List<BedDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<BedDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default);
}