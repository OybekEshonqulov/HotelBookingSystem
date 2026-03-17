using HotelBookingSystem.Application.DTOsNew.AvailabilityNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class AvailabilityService : IAvailabilityService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AvailabilityService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<AvailableRoomDto>> GetAvailableRoomsAsync(AvailabilitySearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate, DateTimeKind.Utc);
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        if (request.CheckOutDate <= request.CheckInDate)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var reservedRoomIds = await _context.ReservationItems
     .Where(x => x.RoomId.HasValue)
     .Where(x => x.Room!.PropertyId == request.PropertyId && x.Room.TenantId == tenantId)
     .Where(x => activeStatuses.Contains(x.Reservation.Status))
     .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
     .Select(x => x.RoomId!.Value)
     .Distinct()
     .ToListAsync(cancellationToken);

        return await _context.Rooms
            .AsNoTracking()
            .Where(x => x.PropertyId == request.PropertyId && x.TenantId == tenantId)
            .Where(x => !reservedRoomIds.Contains(x.Id))
            .Include(x => x.RoomType)
            .OrderBy(x => x.Number)
            .Select(x => new AvailableRoomDto
            {
                RoomId = x.Id,
                RoomNumber = x.Number,
                RoomTypeId = x.RoomTypeId,
                RoomTypeName = x.RoomType.Name,
                Capacity = x.RoomType.Capacity,
                BasePrice = x.RoomType.BasePrice
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AvailableBedDto>> GetAvailableBedsAsync(AvailabilitySearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate, DateTimeKind.Utc);
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        if (request.CheckOutDate <= request.CheckInDate)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var reservedBedIds = await _context.ReservationItems
            .Where(x => x.BedId.HasValue)
            .Where(x => x.Bed!.Room.PropertyId == request.PropertyId && x.Bed.TenantId == tenantId)
            .Where(x => activeStatuses.Contains(x.Reservation.Status))
            .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate).Select(x => x.BedId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _context.Beds
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Room.PropertyId == request.PropertyId)
            .Where(x => !reservedBedIds.Contains(x.Id))
            .Include(x => x.Room)
            .ThenInclude(x => x.RoomType)
            .OrderBy(x => x.Room.Number)
            .ThenBy(x => x.BedCode)
            .Select(x => new AvailableBedDto
            {
                BedId = x.Id,
                BedCode = x.BedCode,
                RoomId = x.RoomId,
                RoomNumber = x.Room.Number,
                RoomTypeId = x.Room.RoomTypeId,
                RoomTypeName = x.Room.RoomType.Name,
                BedPrice = x.BedPrice
            })
            .ToListAsync(cancellationToken);
    }
}