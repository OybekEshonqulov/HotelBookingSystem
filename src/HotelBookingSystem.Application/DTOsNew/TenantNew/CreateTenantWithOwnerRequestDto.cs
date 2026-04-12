namespace HotelBookingSystem.Application.DTOsNew.TenantNew;

public class CreateTenantWithOwnerRequestDto
{
    public string Name { get; set; } = default!;
    public Domain.EnumsNew.TenantType Type { get; set; }
    public string? Subdomain { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string CurrencyCode { get; set; } = "UZS";
    public string TimeZone { get; set; } = "Asia/Tashkent";

    public string OwnerFirstName { get; set; } = default!;
    public string OwnerLastName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public string OwnerPassword { get; set; } = default!;
}