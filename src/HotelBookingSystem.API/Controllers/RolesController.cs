using HotelBookingSystem.Application.DTOsNew.RoleManagementNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleManagementService _roleManagementService;
    private readonly ICurrentUserService _currentUserService;

    public RolesController(
        IRoleManagementService roleManagementService,
        ICurrentUserService currentUserService)
    {
        _roleManagementService = roleManagementService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("roles.view"))
            return Forbid();

        var result = await _roleManagementService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("roles.view"))
            return Forbid();

        var result = await _roleManagementService.GetPermissionsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("roles.create"))
            return Forbid();

        var result = await _roleManagementService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}