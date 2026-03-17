using HotelBookingSystem.Application.DTOsNew.AuthNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken = default);
}