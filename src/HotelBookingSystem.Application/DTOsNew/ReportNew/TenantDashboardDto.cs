namespace HotelBookingSystem.Application.DTOsNew.ReportNew;

public class TenantDashboardDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = default!;
    public int TotalProperties { get; set; }
    public int TotalRooms { get; set; }
    public int TotalBeds { get; set; }
    public int TotalReservations { get; set; }
    public int PendingReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CheckedInReservations { get; set; }
    public int CheckedOutReservations { get; set; }
    public int CancelledReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingAmount { get; set; }
    public decimal OccupancyRate { get; set; }
}