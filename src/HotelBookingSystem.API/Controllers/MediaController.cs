using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly ICurrentUserService _currentUserService;

    public MediaController(IMediaService mediaService, ICurrentUserService currentUserService)
    {
        _mediaService = mediaService;
        _currentUserService = currentUserService;
    }

    [HttpPost("property/{propertyId:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPropertyImage(
        Guid propertyId,
        IFormFile file,
        [FromForm] bool isMain,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("properties.edit"))
            return Forbid();

        var result = await _mediaService.UploadPropertyImageAsync(propertyId, file, isMain, cancellationToken);
        return Ok(result);
    }

    [HttpPost("room/{roomId:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadRoomImage(
        Guid roomId,
        IFormFile file,
        [FromForm] bool isMain,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("properties.edit"))
            return Forbid();

        var result = await _mediaService.UploadRoomImageAsync(roomId, file, isMain, cancellationToken);
        return Ok(result);
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyImages(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await _mediaService.GetPropertyImagesAsync(propertyId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("room/{roomId:guid}")]
    public async Task<IActionResult> GetRoomImages(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _mediaService.GetRoomImagesAsync(roomId, cancellationToken);
        return Ok(result);
    }
}