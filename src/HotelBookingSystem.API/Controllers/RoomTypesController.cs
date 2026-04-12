using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomTypeNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;
    private readonly ICurrentUserService _currentUserService;

    public RoomTypesController(IRoomTypeService roomTypeService, ICurrentUserService currentUserService)
    {
        _roomTypeService = roomTypeService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetByProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _roomTypeService.GetByPropertyAsync(propertyId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomTypeRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesEdit))
            return Forbid();

        var result = await _roomTypeService.CreateAsync(request, cancellationToken);
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

        var result = await _roomTypeService.UpdatePublishStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}