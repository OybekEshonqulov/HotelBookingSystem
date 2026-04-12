using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PropertyNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPropertyService
{
    Task<PropertyDto> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken = default);
    Task<List<PropertyDto>> GetAccessiblePropertiesAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<PropertyDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default);
}