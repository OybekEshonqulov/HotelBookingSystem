namespace HotelBookingSystem.Application.DTOsNew.UserManagementNew;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}