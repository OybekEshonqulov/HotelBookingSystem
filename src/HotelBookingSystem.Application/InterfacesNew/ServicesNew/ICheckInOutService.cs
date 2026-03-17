using HotelBookingSystem.Application.DTOsNew.CheckInOutNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface ICheckInOutService
{
    Task<ReservationStatusResultDto> CheckInAsync(CheckInRequestDto request, CancellationToken cancellationToken = default);
    Task<ReservationStatusResultDto> CheckOutAsync(CheckOutRequestDto request, CancellationToken cancellationToken = default);
}