using HotelBookingSystem.Application.DTOsNew.BedNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BedsController : ControllerBase
{
    private readonly IBedService _bedService;

    public BedsController(IBedService bedService)
    {
        _bedService = bedService;
    }

    [HttpGet("{roomId:guid}")]
    public async Task<IActionResult> GetByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _bedService.GetByRoomAsync(roomId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBedRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _bedService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}