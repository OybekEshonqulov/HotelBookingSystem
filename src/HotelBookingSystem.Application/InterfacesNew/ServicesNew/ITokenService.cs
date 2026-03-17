using HotelBookingSystem.Application.DTOsNew.AuthNew;
using HotelBookingSystem.Domain.EntitiesNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface ITokenService
{
    AuthResponseDto CreateToken(AppUser user, List<string> roles, List<string> permissions, string refreshToken);
    string GenerateRefreshToken();
}