using HotelBookingSystem.Application.DTOsNew.CommonNew;

namespace HotelBookingSystem.Application.DTOsNew.UserManagementNew;

public class UserFilterRequestDto : PagedRequestDto
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}