using HotelBookingSystem.Application.DTOsNew.TenantNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface ITenantService
{
    Task<TenantDto> CreateAsync(CreateTenantRequestDto request, CancellationToken cancellationToken = default);
    Task<TenantWithOwnerResultDto> CreateWithOwnerAsync(CreateTenantWithOwnerRequestDto request, CancellationToken cancellationToken = default);
    Task<List<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDto?> GetMyTenantAsync(CancellationToken cancellationToken = default);
}