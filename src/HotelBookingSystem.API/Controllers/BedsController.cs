using HotelBookingSystem.Application.DTOsNew.BedNew;
using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.InterfacesNew;
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
    private readonly ICurrentUserService _currentUserService;

    public BedsController(IBedService bedService, ICurrentUserService currentUserService)
    {
        _bedService = bedService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{roomId:guid}")]
    public async Task<IActionResult> GetByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _bedService.GetByRoomAsync(roomId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBedRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _bedService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut("{id:guid}/publish")]
    public async Task<IActionResult> UpdatePublishStatus(
    Guid id,
    [FromBody] UpdatePublishStatusRequestDto request,
    CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _bedService.UpdatePublishStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}