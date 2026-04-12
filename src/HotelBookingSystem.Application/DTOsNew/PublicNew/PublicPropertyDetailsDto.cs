using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicPropertyDetailsDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public TenantType Type { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public decimal? AvgRating { get; set; }
    public int ReviewCount { get; set; }

    public List<PublicPropertyImageDto> Images { get; set; } = new();
    public List<PublicPropertyReviewDto> Reviews { get; set; } = new();
    public List<PublicRoomTypeDto> RoomTypes { get; set; } = new();
}