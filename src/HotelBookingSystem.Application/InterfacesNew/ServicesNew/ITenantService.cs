using HotelBookingSystem.Application.DTOsNew.TenantNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface ITenantService
{
    Task<TenantDto> CreateAsync(CreateTenantRequestDto request, CancellationToken cancellationToken = default);
    Task<TenantWithOwnerResultDto> CreateWithOwnerAsync(CreateTenantWithOwnerRequestDto request, CancellationToken cancellationToken = default);

    Task<List<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantDto?> GetMyTenantAsync(CancellationToken cancellationToken = default);

    Task<TenantDto> UpdateAsync(Guid id, UpdateTenantRequestDto request, CancellationToken cancellationToken = default);
    Task<TenantDto> UpdateStatusAsync(Guid id, UpdateTenantStatusRequestDto request, CancellationToken cancellationToken = default);
}