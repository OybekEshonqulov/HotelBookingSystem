using HotelBookingSystem.Application.DTOsNew.MediaNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class MediaService : IMediaService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public MediaService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<ImageDto> UploadPropertyImageAsync(Guid propertyId, IFormFile file, bool isMain, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var property = await _context.Properties
            .FirstOrDefaultAsync(x => x.Id == propertyId && x.TenantId == tenantId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        var savedFile = await _fileStorageService.SaveFileAsync(file, "properties", cancellationToken);

        if (isMain)
        {
            var oldMainImages = await _context.PropertyImages
                .Where(x => x.PropertyId == propertyId && x.IsMain)
                .ToListAsync(cancellationToken);

            foreach (var image in oldMainImages)
                image.IsMain = false;
        }

        var imageEntity = new PropertyImage
        {
            TenantId = tenantId,
            PropertyId = propertyId,
            FileName = savedFile.FileName,
            FilePath = savedFile.FilePath,
            IsMain = isMain
        };

        _context.PropertyImages.Add(imageEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ImageDto
        {
            Id = imageEntity.Id,
            FileName = imageEntity.FileName,
            FilePath = imageEntity.FilePath,
            IsMain = imageEntity.IsMain
        };
    }

    public async Task<ImageDto> UploadRoomImageAsync(Guid roomId, IFormFile file, bool isMain, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var room = await _context.Rooms
            .FirstOrDefaultAsync(x => x.Id == roomId && x.TenantId == tenantId, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        var savedFile = await _fileStorageService.SaveFileAsync(file, "rooms", cancellationToken);

        if (isMain)
        {
            var oldMainImages = await _context.RoomImages
                .Where(x => x.RoomId == roomId && x.IsMain)
                .ToListAsync(cancellationToken);

            foreach (var image in oldMainImages)
                image.IsMain = false;
        }

        var imageEntity = new RoomImage
        {
            TenantId = tenantId,
            RoomId = roomId,
            FileName = savedFile.FileName,
            FilePath = savedFile.FilePath,
            IsMain = isMain
        };

        _context.RoomImages.Add(imageEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ImageDto
        {
            Id = imageEntity.Id,
            FileName = imageEntity.FileName,
            FilePath = imageEntity.FilePath,
            IsMain = imageEntity.IsMain
        };
    }

    public async Task<List<ImageDto>> GetPropertyImagesAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.PropertyImages
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == tenantId)
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => new ImageDto
            {
                Id = x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath,
                IsMain = x.IsMain
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ImageDto>> GetRoomImagesAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.RoomImages
            .AsNoTracking()
            .Where(x => x.RoomId == roomId && x.TenantId == tenantId)
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => new ImageDto
            {
                Id = x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath,
                IsMain = x.IsMain
            })
            .ToListAsync(cancellationToken);
    }
}