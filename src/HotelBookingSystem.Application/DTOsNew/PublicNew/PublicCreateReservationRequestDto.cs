namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicCreateReservationRequestDto
{
    public Guid PropertyId { get; set; }

    public string GuestFirstName { get; set; } = default!;
    public string GuestLastName { get; set; } = default!;
    public string? GuestPhoneNumber { get; set; }
    public string? GuestEmail { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public string? Notes { get; set; }

    public List<PublicCreateReservationItemRequestDto> Items { get; set; } = new();
}