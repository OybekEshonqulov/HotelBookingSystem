using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ICurrentUserService _currentUserService;

    public RoomsController(IRoomService roomService, ICurrentUserService currentUserService)
    {
        _roomService = roomService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetByProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _roomService.GetByPropertyAsync(propertyId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _roomService.CreateAsync(request, cancellationToken);
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

        var result = await _roomService.UpdatePublishStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}