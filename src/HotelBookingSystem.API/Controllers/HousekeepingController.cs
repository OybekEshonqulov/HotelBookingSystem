using HotelBookingSystem.Application.DTOsNew.HousekeepingNew;
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

    public HousekeepingController(IHousekeepingService housekeepingService)
    {
        _housekeepingService = housekeepingService;
    }

    [HttpGet("rooms/{propertyId:guid}")]
    public async Task<IActionResult> GetRooms(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await _housekeepingService.GetRoomsByPropertyAsync(propertyId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("beds/{roomId:guid}")]
    public async Task<IActionResult> GetBeds(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _housekeepingService.GetBedsByRoomAsync(roomId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("room-status")]
    public async Task<IActionResult> UpdateRoomStatus([FromBody] UpdateRoomStatusRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _housekeepingService.UpdateRoomStatusAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("bed-status")]
    public async Task<IActionResult> UpdateBedStatus([FromBody] UpdateBedStatusRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _housekeepingService.UpdateBedStatusAsync(request, cancellationToken);
        return Ok(result);
    }
}