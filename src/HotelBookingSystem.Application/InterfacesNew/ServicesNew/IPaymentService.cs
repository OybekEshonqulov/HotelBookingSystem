using HotelBookingSystem.Application.DTOsNew.PaymentNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<List<PaymentDto>> GetByReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
}