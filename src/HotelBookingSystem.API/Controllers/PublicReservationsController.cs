using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/public/reservations")]
[AllowAnonymous]
public class PublicReservationsController : ControllerBase
{
    private readonly IPublicBookingService _publicBookingService;

    public PublicReservationsController(IPublicBookingService publicBookingService)
    {
        _publicBookingService = publicBookingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] PublicCreateReservationRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _publicBookingService.CreateReservationAsync(request, cancellationToken);
        return Ok(result);
    }
}