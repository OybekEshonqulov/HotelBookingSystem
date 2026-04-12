namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    List<string> Roles { get; }
    List<string> Permissions { get; }
}