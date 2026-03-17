using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public MeController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public IActionResult GetMe()
    {
        return Ok(new
        {
            userId = _currentUserService.UserId,
            tenantId = _currentUserService.TenantId,
            email = _currentUserService.Email,
            isAuthenticated = _currentUserService.IsAuthenticated,
            roles = _currentUserService.Roles,
            permissions = _currentUserService.Permissions
        });
    }
}