namespace HotelBookingSystem.Application.DTOsNew.PublicNew;

public class PublicPropertyImageDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public bool IsMain { get; set; }
}