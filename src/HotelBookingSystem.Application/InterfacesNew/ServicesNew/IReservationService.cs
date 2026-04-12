using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.ReservationNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(CreateReservationRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<ReservationDto>> GetPagedAsync(ReservationFilterRequestDto request, CancellationToken cancellationToken = default);
    Task<ReservationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReservationDto> ChangeStatusAsync(Guid id, ChangeReservationStatusRequestDto request, CancellationToken cancellationToken = default);
}