using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/public/properties")]
[AllowAnonymous]
public class PublicPropertiesController : ControllerBase
{
    private readonly IPublicCatalogService _publicCatalogService;

    public PublicPropertiesController(IPublicCatalogService publicCatalogService)
    {
        _publicCatalogService = publicCatalogService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] PublicPropertyFilterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetPropertiesAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetPropertyByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}