using HotelBookingSystem.Application.DTOsNew.HousekeepingNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HousekeepingController : ControllerBase
{
    private readonly IHousekeepingService _housekeepingService;
    private readonly ICurrentUserService _currentUserService;

    public HousekeepingController(
        IHousekeepingService housekeepingService,
        ICurrentUserService currentUserService)
    {
        _housekeepingService = housekeepingService;
        _currentUserService = currentUserService;
    }

    [HttpGet("rooms/{propertyId:guid}")]
    public async Task<IActionResult> GetRooms(Guid propertyId, [FromQuery] Guid? tenantId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _housekeepingService.GetRoomsByPropertyAsync(propertyId, tenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("beds/{roomId:guid}")]
    public async Task<IActionResult> GetBeds(Guid roomId, [FromQuery] Guid? tenantId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _housekeepingService.GetBedsByRoomAsync(roomId, tenantId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("room-status")]
    public async Task<IActionResult> UpdateRoomStatus([FromBody] UpdateRoomStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _housekeepingService.UpdateRoomStatusAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("bed-status")]
    public async Task<IActionResult> UpdateBedStatus([FromBody] UpdateBedStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _housekeepingService.UpdateBedStatusAsync(request, cancellationToken);
        return Ok(result);
    }
}