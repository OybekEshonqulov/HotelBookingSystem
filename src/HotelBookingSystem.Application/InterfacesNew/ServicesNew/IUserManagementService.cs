using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.UserManagementNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IUserManagementService
{
    Task<UserDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterRequestDto request, CancellationToken cancellationToken = default);
    Task<UserDto> AssignRolesAsync(AssignRolesRequestDto request, CancellationToken cancellationToken = default);
}