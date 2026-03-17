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

    public ReportsController(
        IReportService reportService,
        ICurrentUserService currentUserService)
    {
        _reportService = reportService;
        _currentUserService = currentUserService;
    }

    [HttpGet("dashboard/{propertyId:guid}")]
    public async Task<IActionResult> GetDashboard(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reports.view"))
            return Forbid();

        var result = await _reportService.GetDashboardStatsAsync(propertyId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("occupancy/{propertyId:guid}")]
    public async Task<IActionResult> GetOccupancy(
        Guid propertyId,
        [FromQuery] DateTime checkInDate,
        [FromQuery] DateTime checkOutDate,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reports.view"))
            return Forbid();

        var result = await _reportService.GetOccupancyAsync(propertyId, checkInDate, checkOutDate, cancellationToken);
        return Ok(result);
    }
}