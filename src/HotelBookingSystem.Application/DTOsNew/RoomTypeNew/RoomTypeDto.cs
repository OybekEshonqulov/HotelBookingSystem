namespace HotelBookingSystem.Application.DTOsNew.RoomTypeNew;

public class RoomTypeDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PropertyId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsPublished { get; set; }
}