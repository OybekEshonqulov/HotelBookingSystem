using HotelBookingSystem.Domain.CommonNew;

namespace HotelBookingSystem.Domain.EntitiesNew;

public class PropertyImage : TenantEntity
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public bool IsMain { get; set; } = false;
}