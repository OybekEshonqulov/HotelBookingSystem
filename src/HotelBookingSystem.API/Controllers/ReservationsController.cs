using HotelBookingSystem.Application.DTOsNew.ReservationNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ICurrentUserService _currentUserService;

    public ReservationsController(IReservationService reservationService, ICurrentUserService currentUserService)
    {
        _reservationService = reservationService;
        _currentUserService = currentUserService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> GetPaged([FromBody] ReservationFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.ReservationsView))
            return Forbid();

        var result = await _reservationService.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.ReservationsView))
            return Forbid();

        var result = await _reservationService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.ReservationsCreate))
            return Forbid();

        var result = await _reservationService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeReservationStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.ReservationsEdit))
            return Forbid();

        var result = await _reservationService.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }
}