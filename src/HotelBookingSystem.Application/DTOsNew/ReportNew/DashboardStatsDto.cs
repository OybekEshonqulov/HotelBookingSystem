namespace HotelBookingSystem.Application.DTOsNew.ReportNew;

public class DashboardStatsDto
{
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CheckedInReservations { get; set; }
    public int CheckedOutReservations { get; set; }
    public int CancelledReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDue { get; set; }
}