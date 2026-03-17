namespace HotelBookingSystem.Application.DTOsNew.UserManagementNew;

public class AssignRolesRequestDto
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}