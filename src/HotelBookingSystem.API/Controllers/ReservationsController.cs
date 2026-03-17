using HotelBookingSystem.Application.DTOsNew.ReservationActionNew;
using HotelBookingSystem.Application.DTOsNew.ReservationNew;
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

    public ReservationsController(
        IReservationService reservationService,
        ICurrentUserService currentUserService)
    {
        _reservationService = reservationService;
        _currentUserService = currentUserService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> GetPaged([FromBody] ReservationFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reservations.view"))
            return Forbid();

        var result = await _reservationService.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reservations.view"))
            return Forbid();

        var result = await _reservationService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reservations.create"))
            return Forbid();

        var result = await _reservationService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel([FromBody] CancelReservationRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("reservations.cancel"))
            return Forbid();

        var result = await _reservationService.CancelAsync(request, cancellationToken);
        return Ok(result);
    }
}