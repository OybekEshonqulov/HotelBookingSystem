using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.RoomNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RoomService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<RoomDto> CreateAsync(CreateRoomRequestDto request, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        EnsureTenantAccess(property.TenantId);

        var roomTypeExists = await _context.RoomTypes.AnyAsync(x =>
            x.Id == request.RoomTypeId &&
            x.PropertyId == request.PropertyId &&
            x.TenantId == property.TenantId,
            cancellationToken);

        if (!roomTypeExists)
            throw new NotFoundException("Room type topilmadi.");

        var exists = await _context.Rooms.AnyAsync(x =>
            x.PropertyId == request.PropertyId &&
            x.TenantId == property.TenantId &&
            x.Number == request.Number.Trim(),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu raqamdagi xona allaqachon mavjud.");

        var room = new Room
        {
            TenantId = property.TenantId,
            PropertyId = request.PropertyId,
            RoomTypeId = request.RoomTypeId,
            Number = request.Number.Trim(),
            Floor = request.Floor,
            IsPublished = false
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync(cancellationToken);

        return Map(room);
    }

    public async Task<List<RoomDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        EnsureTenantAccess(property.TenantId);

        return await _context.Rooms
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == property.TenantId)
            .OrderBy(x => x.Number)
            .Select(x => new RoomDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                RoomTypeId = x.RoomTypeId,
                Number = x.Number,
                Floor = x.Floor,
                IsPublished = x.IsPublished
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RoomDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        EnsureTenantAccess(room.TenantId);

        room.IsPublished = request.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        return Map(room);
    }

    private void EnsureTenantAccess(Guid tenantId)
    {
        if (_currentUserService.IsSuperAdmin)
            return;

        if (!_currentUserService.TenantId.HasValue || _currentUserService.TenantId.Value != tenantId)
            throw new NotFoundException("Property topilmadi.");
    }

    private static RoomDto Map(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            TenantId = room.TenantId,
            PropertyId = room.PropertyId,
            RoomTypeId = room.RoomTypeId,
            Number = room.Number,
            Floor = room.Floor,
            IsPublished = room.IsPublished
        };
    }
}