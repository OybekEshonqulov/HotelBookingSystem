namespace HotelBookingSystem.Application.DTOsNew.UserManagementNew;

public class CreateUserRequestDto
{
    public Guid? TenantId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public List<Guid> RoleIds { get; set; } = new();
}