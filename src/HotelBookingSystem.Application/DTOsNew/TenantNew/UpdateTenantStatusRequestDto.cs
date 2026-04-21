using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.TenantNew;

public class UpdateTenantStatusRequestDto
{
    public PropertyStatus Status { get; set; }
}