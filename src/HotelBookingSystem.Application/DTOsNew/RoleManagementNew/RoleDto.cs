namespace HotelBookingSystem.Application.DTOsNew.RoleManagementNew;

public class RoleDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
}