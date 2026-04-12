using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomTypeNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class RoomTypeService : IRoomTypeService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RoomTypeService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<RoomTypeDto> CreateAsync(CreateRoomTypeRequestDto request, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        EnsureTenantAccess(property.TenantId);

        var exists = await _context.RoomTypes.AnyAsync(x =>
            x.PropertyId == request.PropertyId &&
            x.TenantId == property.TenantId &&
            x.Name == request.Name.Trim(),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi room type allaqachon mavjud.");

        var roomType = new Domain.EntitiesNew.RoomType
        {
            TenantId = property.TenantId,
            PropertyId = request.PropertyId,
            Name = request.Name.Trim(),
            Capacity = request.Capacity,
            BasePrice = request.BasePrice,
            IsPublished = false
        };

        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync(cancellationToken);

        return Map(roomType);
    }

    public async Task<List<RoomTypeDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        EnsureTenantAccess(property.TenantId);

        return await _context.RoomTypes
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == property.TenantId)
            .OrderBy(x => x.Name)
            .Select(x => new RoomTypeDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                Name = x.Name,
                Capacity = x.Capacity,
                BasePrice = x.BasePrice,
                IsPublished = x.IsPublished
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RoomTypeDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var roomType = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (roomType is null)
            throw new NotFoundException("Room type topilmadi.");

        EnsureTenantAccess(roomType.TenantId);

        roomType.IsPublished = request.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        return Map(roomType);
    }

    private void EnsureTenantAccess(Guid tenantId)
    {
        if (_currentUserService.IsSuperAdmin)
            return;

        if (!_currentUserService.TenantId.HasValue || _currentUserService.TenantId.Value != tenantId)
            throw new NotFoundException("Property topilmadi.");
    }

    private static RoomTypeDto Map(Domain.EntitiesNew.RoomType roomType)
    {
        return new RoomTypeDto
        {
            Id = roomType.Id,
            TenantId = roomType.TenantId,
            PropertyId = roomType.PropertyId,
            Name = roomType.Name,
            Capacity = roomType.Capacity,
            BasePrice = roomType.BasePrice,
            IsPublished = roomType.IsPublished
        };
    }
}