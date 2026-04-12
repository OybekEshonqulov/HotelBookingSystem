using HotelBookingSystem.Application.DTOsNew.UserManagementNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(
        IUserManagementService userManagementService,
        ICurrentUserService currentUserService)
    {
        _userManagementService = userManagementService;
        _currentUserService = currentUserService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> GetPaged([FromBody] UserFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.view"))
            return Forbid();

        var result = await _userManagementService.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.view"))
            return Forbid();

        var result = await _userManagementService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.create"))
            return Forbid();

        var result = await _userManagementService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.edit"))
            return Forbid();

        var result = await _userManagementService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.edit"))
            return Forbid();

        var result = await _userManagementService.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("assign-roles")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRolesRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("users.edit"))
            return Forbid();

        var result = await _userManagementService.AssignRolesAsync(request, cancellationToken);
        return Ok(result);
    }
}