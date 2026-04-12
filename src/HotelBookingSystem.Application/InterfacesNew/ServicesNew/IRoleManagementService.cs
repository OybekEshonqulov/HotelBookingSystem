using HotelBookingSystem.Application.DTOsNew.RoleManagementNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IRoleManagementService
{
    Task<RoleDto> CreateAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default);
    Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}