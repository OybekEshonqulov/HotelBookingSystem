using HotelBookingSystem.Application.DTOsNew.ReportNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IReportService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<OccupancyReportDto> GetOccupancyAsync(Guid propertyId, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken = default);
}