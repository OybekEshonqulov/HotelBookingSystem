namespace HotelBookingSystem.Domain.EntitiesNew;

public class PropertyReview
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Guid UserId { get; set; }

    public int Rating { get; set; } // 1..5
    public string? Comment { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Property Property { get; set; } = default!;
    public User User { get; set; } = default!;
}