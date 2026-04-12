using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PropertyNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ICurrentUserService _currentUserService;

    public PropertiesController(IPropertyService propertyService, ICurrentUserService currentUserService)
    {
        _propertyService = propertyService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccessibleProperties([FromQuery] Guid? tenantId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesView))
            return Forbid();

        var result = await _propertyService.GetAccessiblePropertiesAsync(tenantId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PropertiesCreate))
            return Forbid();

        var result = await _propertyService.CreateAsync(request, cancellationToken);
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

        var result = await _propertyService.UpdatePublishStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}