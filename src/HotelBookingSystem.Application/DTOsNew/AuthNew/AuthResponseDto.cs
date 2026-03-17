namespace HotelBookingSystem.Application.DTOsNew.AuthNew;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}