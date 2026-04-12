using HotelBookingSystem.Application.DTOsNew.ReportNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ICurrentUserService _currentUserService;

    public ReportsController(IReportService reportService, ICurrentUserService currentUserService)
    {
        _reportService = reportService;
        _currentUserService = currentUserService;
    }

    [HttpPost("tenant-dashboard")]
    public async Task<IActionResult> GetTenantDashboard(
        [FromBody] TenantDashboardRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.ReportsView))
            return Forbid();

        var result = await _reportService.GetTenantDashboardAsync(request, cancellationToken);
        return Ok(result);
    }
}