namespace HotelBookingSystem.Application.DTOsNew.UserManagementNew;

public class UpdateUserRequestDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
}