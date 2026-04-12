using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicPropertyFilterRequestDto
{
    public string? Search { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public TenantType? Type { get; set; }

    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }

    public int? GuestsCount { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}