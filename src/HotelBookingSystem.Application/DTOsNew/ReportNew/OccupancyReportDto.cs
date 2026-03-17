namespace HotelBookingSystem.Application.DTOsNew.ReportNew;

public class OccupancyReportDto
{
    public Guid PropertyId { get; set; }
    public int TotalRooms { get; set; }
    public int ReservedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalBeds { get; set; }
    public int ReservedBeds { get; set; }
    public int AvailableBeds { get; set; }
}