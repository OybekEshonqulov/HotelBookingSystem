using HotelBookingSystem.Domain.CommonNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class RoomImage : TenantEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public bool IsMain { get; set; } = false;
}