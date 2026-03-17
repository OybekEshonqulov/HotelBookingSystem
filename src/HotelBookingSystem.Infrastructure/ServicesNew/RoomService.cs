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
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var propertyExists = await _context.Properties.AnyAsync(x =>
            x.Id == request.PropertyId &&
            x.TenantId == _currentUserService.TenantId.Value,
            cancellationToken);

        if (!propertyExists)
            throw new NotFoundException("Property topilmadi.");

        var roomTypeExists = await _context.RoomTypes.AnyAsync(x =>
            x.Id == request.RoomTypeId &&
            x.PropertyId == request.PropertyId &&
            x.TenantId == _currentUserService.TenantId.Value,
            cancellationToken);

        if (!roomTypeExists)
            throw new NotFoundException("Room type topilmadi.");

        var exists = await _context.Rooms.AnyAsync(x =>
            x.PropertyId == request.PropertyId &&
            x.TenantId == _currentUserService.TenantId.Value &&
            x.Number == request.Number,
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu raqamdagi xona allaqachon mavjud.");

        var room = new Room
        {
            TenantId = _currentUserService.TenantId.Value,
            PropertyId = request.PropertyId,
            RoomTypeId = request.RoomTypeId,
            Number = request.Number,
            Floor = request.Floor
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync(cancellationToken);

        return new RoomDto
        {
            Id = room.Id,
            TenantId = room.TenantId,
            PropertyId = room.PropertyId,
            RoomTypeId = room.RoomTypeId,
            Number = room.Number,
            Floor = room.Floor
        };
    }

    public async Task<List<RoomDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return await _context.Rooms
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == _currentUserService.TenantId.Value)
            .OrderBy(x => x.Number)
            .Select(x => new RoomDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                RoomTypeId = x.RoomTypeId,
                Number = x.Number,
                Floor = x.Floor
            })
            .ToListAsync(cancellationToken);
    }
}