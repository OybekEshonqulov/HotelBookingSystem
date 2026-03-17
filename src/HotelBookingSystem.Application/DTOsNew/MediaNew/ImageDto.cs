namespace HotelBookingSystem.Application.DTOsNew.MediaNew;

public class ImageDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public bool IsMain { get; set; }
}