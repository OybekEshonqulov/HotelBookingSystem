using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<(string FileName, string FilePath)> SaveFileAsync(
        IFormFile file,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Fayl topilmadi.");

        var uploadsRoot = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", folderName);

        if (!Directory.Exists(uploadsRoot))
            Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(file.FileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsRoot, newFileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var relativePath = $"/uploads/{folderName}/{newFileName}";
        return (newFileName, relativePath);
    }
}