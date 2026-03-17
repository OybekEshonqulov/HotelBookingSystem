using HotelBookingSystem.Application.DTOsNew.CheckInOutNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInOutController : ControllerBase
{
    private readonly ICheckInOutService _checkInOutService;

    public CheckInOutController(ICheckInOutService checkInOutService)
    {
        _checkInOutService = checkInOutService;
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _checkInOutService.CheckInAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _checkInOutService.CheckOutAsync(request, cancellationToken);
        return Ok(result);
    }
}