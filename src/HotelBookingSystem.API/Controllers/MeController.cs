using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly AppDbContext _context;

    public MeController(ICurrentUserService currentUserService, AppDbContext context)
    {
        _currentUserService = currentUserService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        string? tenantName = null;

        if (_currentUserService.TenantId.HasValue)
        {
            tenantName = await _context.Tenants
                .AsNoTracking()
                .Where(x => x.Id == _currentUserService.TenantId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return Ok(new
        {
            userId = _currentUserService.UserId,
            tenantId = _currentUserService.TenantId,
            tenantName,
            email = _currentUserService.Email,
            isAuthenticated = _currentUserService.IsAuthenticated,
            roles = _currentUserService.Roles,
            permissions = _currentUserService.Permissions
        });
    }
}