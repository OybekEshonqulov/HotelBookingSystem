using HotelBookingSystem.Application.DTOsNew.HousekeepingNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class HousekeepingService : IHousekeepingService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public HousekeepingService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<RoomStatusDto> UpdateRoomStatusAsync(UpdateRoomStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var room = await _context.Rooms
            .FirstOrDefaultAsync(x => x.Id == request.RoomId && x.TenantId == tenantId, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        room.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return new RoomStatusDto
        {
            RoomId = room.Id,
            RoomNumber = room.Number,
            Status = room.Status
        };
    }

    public async Task<BedStatusDto> UpdateBedStatusAsync(UpdateBedStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var bed = await _context.Beds
            .FirstOrDefaultAsync(x => x.Id == request.BedId && x.TenantId == tenantId, cancellationToken);

        if (bed is null)
            throw new NotFoundException("Bed topilmadi.");

        bed.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return new BedStatusDto
        {
            BedId = bed.Id,
            BedCode = bed.BedCode,
            Status = bed.Status
        };
    }

    public async Task<List<RoomStatusDto>> GetRoomsByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.Rooms
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == tenantId)
            .OrderBy(x => x.Number)
            .Select(x => new RoomStatusDto
            {
                RoomId = x.Id,
                RoomNumber = x.Number,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BedStatusDto>> GetBedsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.Beds
            .AsNoTracking()
            .Where(x => x.RoomId == roomId && x.TenantId == tenantId)
            .OrderBy(x => x.BedCode)
            .Select(x => new BedStatusDto
            {
                BedId = x.Id,
                BedCode = x.BedCode,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }
}