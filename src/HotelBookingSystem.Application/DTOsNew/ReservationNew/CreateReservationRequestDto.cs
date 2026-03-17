using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.ReservationNew;

public class CreateReservationRequestDto
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

    public ReservationSource Source { get; set; } = ReservationSource.AdminPanel;
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public string? Notes { get; set; }

    public List<CreateReservationItemRequestDto> Items { get; set; } = new();
}
