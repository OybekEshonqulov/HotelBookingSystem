using HotelBookingSystem.Application.DTOsNew.BedNew;
using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class BedService : IBedService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BedService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<BedDto> CreateAsync(CreateBedRequestDto request, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        EnsureTenantAccess(room.TenantId);

        var exists = await _context.Beds.AnyAsync(x =>
            x.RoomId == request.RoomId &&
            x.TenantId == room.TenantId &&
            x.BedCode == request.BedCode.Trim(),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu koddagi bed allaqachon mavjud.");

        var bed = new Bed
        {
            TenantId = room.TenantId,
            RoomId = request.RoomId,
            BedCode = request.BedCode.Trim(),
            BedPrice = request.BedPrice,
            IsPublished = false
        };

        _context.Beds.Add(bed);
        await _context.SaveChangesAsync(cancellationToken);

        return Map(bed);
    }

    public async Task<List<BedDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        EnsureTenantAccess(room.TenantId);

        return await _context.Beds
            .AsNoTracking()
            .Where(x => x.RoomId == roomId && x.TenantId == room.TenantId)
            .OrderBy(x => x.BedCode)
            .Select(x => new BedDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                RoomId = x.RoomId,
                BedCode = x.BedCode,
                BedPrice = x.BedPrice,
                IsPublished = x.IsPublished
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BedDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var bed = await _context.Beds.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (bed is null)
            throw new NotFoundException("Bed topilmadi.");

        EnsureTenantAccess(bed.TenantId);

        bed.IsPublished = request.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        return Map(bed);
    }

    private void EnsureTenantAccess(Guid tenantId)
    {
        if (_currentUserService.IsSuperAdmin)
            return;

        if (!_currentUserService.TenantId.HasValue || _currentUserService.TenantId.Value != tenantId)
            throw new NotFoundException("Room topilmadi.");
    }

    private static BedDto Map(Bed bed)
    {
        return new BedDto
        {
            Id = bed.Id,
            TenantId = bed.TenantId,
            RoomId = bed.RoomId,
            BedCode = bed.BedCode,
            BedPrice = bed.BedPrice,
            IsPublished = bed.IsPublished
        };
    }
}