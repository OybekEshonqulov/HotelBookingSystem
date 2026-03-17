using Microsoft.AspNetCore.Http;
namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IFileStorageService
{
    Task<(string FileName, string FilePath)> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
}