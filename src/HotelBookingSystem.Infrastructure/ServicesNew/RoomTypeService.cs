using HotelBookingSystem.Application.DTOsNew.RoomTypeNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
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
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == request.PropertyId &&
                x.TenantId == _currentUserService.TenantId.Value,
                cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        var exists = await _context.RoomTypes.AnyAsync(x =>
            x.PropertyId == request.PropertyId &&
            x.TenantId == _currentUserService.TenantId.Value &&
            x.Name == request.Name,
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi room type allaqachon mavjud.");

        var roomType = new Domain.EntitiesNew.RoomType
        {
            TenantId = _currentUserService.TenantId.Value,
            PropertyId = request.PropertyId,
            Name = request.Name,
            Capacity = request.Capacity,
            BasePrice = request.BasePrice
        };

        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync(cancellationToken);

        return new RoomTypeDto
        {
            Id = roomType.Id,
            TenantId = roomType.TenantId,
            PropertyId = roomType.PropertyId,
            Name = roomType.Name,
            Capacity = roomType.Capacity,
            BasePrice = roomType.BasePrice
        };
    }

    public async Task<List<RoomTypeDto>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return await _context.RoomTypes
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId && x.TenantId == _currentUserService.TenantId.Value)
            .OrderBy(x => x.Name)
            .Select(x => new RoomTypeDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                Name = x.Name,
                Capacity = x.Capacity,
                BasePrice = x.BasePrice
            })
            .ToListAsync(cancellationToken);
    }
}