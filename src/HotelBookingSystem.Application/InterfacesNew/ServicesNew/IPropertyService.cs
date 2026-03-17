using HotelBookingSystem.Application.DTOsNew.PropertyNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPropertyService
{
    Task<PropertyDto> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken = default);
    Task<List<PropertyDto>> GetMyPropertiesAsync(CancellationToken cancellationToken = default);
}