using HotelBookingSystem.Domain.CommonNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class Guest : TenantEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsBlacklisted { get; set; } = false;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}