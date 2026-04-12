using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicPropertyCardDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public TenantType Type { get; set; }
    public string? MainImageUrl { get; set; }

    public decimal MinPrice { get; set; }
    public decimal? AvgRating { get; set; }
    public int ReviewCount { get; set; }
}