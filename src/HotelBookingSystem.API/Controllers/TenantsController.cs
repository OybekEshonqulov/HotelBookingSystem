using HotelBookingSystem.Application.DTOsNew.TenantNew;
using HotelBookingSystem.Application.InterfacesNew;
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
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsView))
            return Forbid();

        var result = await _tenantService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsView))
            return Forbid();

        var result = await _tenantService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyTenant(CancellationToken cancellationToken)
    {
        var result = await _tenantService.GetMyTenantAsync(cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsCreate))
            return Forbid();

        var result = await _tenantService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create-with-owner")]
    public async Task<IActionResult> CreateWithOwner(
        [FromBody] CreateTenantWithOwnerRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsCreate))
            return Forbid();

        var result = await _tenantService.CreateWithOwnerAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsEdit))
            return Forbid();

        var result = await _tenantService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTenantStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.TenantsEdit))
            return Forbid();

        var result = await _tenantService.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}