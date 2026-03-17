namespace HotelBookingSystem.Application.DTOsNew.RoleManagementNew;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}