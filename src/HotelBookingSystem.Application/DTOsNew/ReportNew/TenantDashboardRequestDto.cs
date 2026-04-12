namespace HotelBookingSystem.Application.DTOsNew.ReportNew;

public class TenantDashboardRequestDto
{
    public Guid? TenantId { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}