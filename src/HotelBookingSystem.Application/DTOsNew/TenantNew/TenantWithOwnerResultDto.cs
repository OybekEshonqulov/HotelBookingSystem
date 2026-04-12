namespace HotelBookingSystem.Application.DTOsNew.TenantNew;

public class TenantWithOwnerResultDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = default!;
    public string TenantSubdomain { get; set; } = default!;

    public Guid OwnerUserId { get; set; }
    public string OwnerFullName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;

    public Guid OwnerRoleId { get; set; }
    public string OwnerRoleName { get; set; } = default!;
}