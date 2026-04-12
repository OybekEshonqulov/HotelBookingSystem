namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicPropertyReviewDto
{
    public Guid Id { get; set; }
    public string UserFullName { get; set; } = default!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}