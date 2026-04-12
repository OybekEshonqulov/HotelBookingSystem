using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/public/availability")]
[AllowAnonymous]
public class PublicAvailabilityController : ControllerBase
{
    private readonly IPublicCatalogService _publicCatalogService;

    public PublicAvailabilityController(IPublicCatalogService publicCatalogService)
    {
        _publicCatalogService = publicCatalogService;
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> GetAvailableRooms(
        [FromBody] PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetAvailableRoomsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("beds")]
    public async Task<IActionResult> GetAvailableBeds(
        [FromBody] PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetAvailableBedsAsync(request, cancellationToken);
        return Ok(result);
    }
}