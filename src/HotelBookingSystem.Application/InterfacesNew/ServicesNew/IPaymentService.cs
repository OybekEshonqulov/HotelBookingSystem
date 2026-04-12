using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PaymentNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<PaymentDto>> GetPagedAsync(PaymentFilterRequestDto request, CancellationToken cancellationToken = default);
}