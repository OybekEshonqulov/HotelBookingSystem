using HotelBookingSystem.Application.DTOsNew.AvailabilityNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> GetAvailableRooms([FromBody] AvailabilitySearchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _availabilityService.GetAvailableRoomsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("beds")]
    public async Task<IActionResult> GetAvailableBeds([FromBody] AvailabilitySearchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _availabilityService.GetAvailableBedsAsync(request, cancellationToken);
        return Ok(result);
    }
}