using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.TenantNew;

public class CreateTenantRequestDto
{
    public string Name { get; set; } = default!;
    public TenantType Type { get; set; }
    public string? Subdomain { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string CurrencyCode { get; set; } = "UZS";
    public string TimeZone { get; set; } = "Asia/Tashkent";
}