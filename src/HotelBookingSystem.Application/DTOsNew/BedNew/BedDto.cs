namespace HotelBookingSystem.Application.DTOsNew.BedNew;

public class BedDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoomId { get; set; }
    public string BedCode { get; set; } = default!;
    public decimal? BedPrice { get; set; }
}