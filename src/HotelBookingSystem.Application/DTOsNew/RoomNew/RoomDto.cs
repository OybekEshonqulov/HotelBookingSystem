namespace HotelBookingSystem.Application.DTOsNew.RoomNew;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Number { get; set; } = default!;
    public int Floor { get; set; }
}