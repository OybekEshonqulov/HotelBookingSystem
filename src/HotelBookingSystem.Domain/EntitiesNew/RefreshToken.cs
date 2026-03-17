using HotelBookingSystem.Domain.CommonNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class RefreshToken : TenantEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = default!;

    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAtUtc { get; set; }
}