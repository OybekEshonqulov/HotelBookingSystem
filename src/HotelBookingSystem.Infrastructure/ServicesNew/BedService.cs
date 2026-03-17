using HotelBookingSystem.Application.DTOsNew.BedNew;
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
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == request.RoomId &&
                x.TenantId == _currentUserService.TenantId.Value,
                cancellationToken);

        if (room is null)
            throw new NotFoundException("Room topilmadi.");

        var exists = await _context.Beds.AnyAsync(x =>
            x.RoomId == request.RoomId &&
            x.TenantId == _currentUserService.TenantId.Value &&
            x.BedCode == request.BedCode,
            cancellationToken);

        if (exists)
            throw new NotFoundException("Bu koddagi bed allaqachon mavjud.");

        var bed = new Bed
        {
            TenantId = _currentUserService.TenantId.Value,
            RoomId = request.RoomId,
            BedCode = request.BedCode,
            BedPrice = request.BedPrice
        };

        _context.Beds.Add(bed);
        await _context.SaveChangesAsync(cancellationToken);

        return new BedDto
        {
            Id = bed.Id,
            TenantId = bed.TenantId,
            RoomId = bed.RoomId,
            BedCode = bed.BedCode,
            BedPrice = bed.BedPrice
        };
    }

    public async Task<List<BedDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        return await _context.Beds
            .AsNoTracking()
            .Where(x => x.RoomId == roomId && x.TenantId == _currentUserService.TenantId.Value)
            .OrderBy(x => x.BedCode)
            .Select(x => new BedDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                RoomId = x.RoomId,
                BedCode = x.BedCode,
                BedPrice = x.BedPrice
            })
            .ToListAsync(cancellationToken);
    }
}