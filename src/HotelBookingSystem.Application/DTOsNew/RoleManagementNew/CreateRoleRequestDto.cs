namespace HotelBookingSystem.Application.DTOsNew.RoleManagementNew;

public class CreateRoleRequestDto
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}