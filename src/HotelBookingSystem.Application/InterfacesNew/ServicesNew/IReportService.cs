using HotelBookingSystem.Application.DTOsNew.ReportNew;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IReportService
{
    Task<TenantDashboardDto> GetTenantDashboardAsync(TenantDashboardRequestDto request, CancellationToken cancellationToken = default);
    Task<SystemDashboardDto> GetSystemDashboardAsync(SystemDashboardRequestDto request, CancellationToken cancellationToken = default);
}