using HotelBookingSystem.Application.DTOsNew.PropertyNew;
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
    public async Task<IActionResult> GetMyProperties(CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("properties.view"))
            return Forbid();
        var result = await _propertyService.GetMyPropertiesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("properties.create"))
            return Forbid();

        var result = await _propertyService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}