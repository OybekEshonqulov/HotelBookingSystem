using HotelBookingSystem.Application.DTOsNew.TenantNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public TenantsController(ITenantService tenantService, ICurrentUserService currentUserService)
    {
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _tenantService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("tenants.create"))
            return Forbid();

        var result = await _tenantService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}