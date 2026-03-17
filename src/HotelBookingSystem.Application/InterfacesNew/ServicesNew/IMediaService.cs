using HotelBookingSystem.Application.DTOsNew.MediaNew;
using Microsoft.AspNetCore.Http;

namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IMediaService
{
    Task<ImageDto> UploadPropertyImageAsync(Guid propertyId, IFormFile file, bool isMain, CancellationToken cancellationToken = default);
    Task<ImageDto> UploadRoomImageAsync(Guid roomId, IFormFile file, bool isMain, CancellationToken cancellationToken = default);
    Task<List<ImageDto>> GetPropertyImagesAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<List<ImageDto>> GetRoomImagesAsync(Guid roomId, CancellationToken cancellationToken = default);
}