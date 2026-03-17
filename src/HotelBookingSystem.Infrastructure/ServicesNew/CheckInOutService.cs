using HotelBookingSystem.Application.DTOsNew.CheckInOutNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class CheckInOutService : ICheckInOutService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckInOutService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ReservationStatusResultDto> CheckInAsync(CheckInRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var reservation = await _context.Reservations
     .Include(x => x.Items)
     .FirstOrDefaultAsync(x => x.Id == request.ReservationId && x.TenantId == tenantId, cancellationToken);

        if (reservation is null)
            throw new NotFoundException("Reservation topilmadi.");

        if (reservation.Status != ReservationStatus.Confirmed && reservation.Status != ReservationStatus.Pending)
            throw new BadRequestException("Faqat confirmed yoki pending reservation check-in qilinadi.");

        reservation.Status = ReservationStatus.CheckedIn;
        var roomIds = reservation.Items
    .Where(x => x.RoomId.HasValue)
    .Select(x => x.RoomId!.Value)
    .Distinct()
    .ToList();

        var bedIds = reservation.Items
            .Where(x => x.BedId.HasValue)
            .Select(x => x.BedId!.Value)
            .Distinct()
            .ToList();

        if (roomIds.Count > 0)
        {
            var rooms = await _context.Rooms.Where(x => roomIds.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var room in rooms)
                room.Status = HotelBookingSystem.Domain.EnumsNew.RoomStatus.Occupied;
        }

        if (bedIds.Count > 0)
        {
            var beds = await _context.Beds.Where(x => bedIds.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var bed in beds)
                bed.Status = HotelBookingSystem.Domain.EnumsNew.BedStatus.Occupied;
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            reservation.Notes = string.IsNullOrWhiteSpace(reservation.Notes)
                ? request.Notes
                : reservation.Notes + " | CheckIn: " + request.Notes;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ReservationStatusResultDto
        {
            ReservationId = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            Status = reservation.Status,
            TotalAmount = reservation.TotalAmount,
            PaidAmount = reservation.PaidAmount,
            DueAmount = reservation.TotalAmount - reservation.PaidAmount
        };
    }

    public async Task<ReservationStatusResultDto> CheckOutAsync(CheckOutRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var reservation = await _context.Reservations.Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.ReservationId && x.TenantId == tenantId, cancellationToken);

        if (reservation is null)
            throw new NotFoundException("Reservation topilmadi.");

        if (reservation.Status != ReservationStatus.CheckedIn)
            throw new BadRequestException("Faqat checked-in reservation check-out qilinadi.");

        reservation.Status = ReservationStatus.CheckedOut;

        var roomIds = reservation.Items
    .Where(x => x.RoomId.HasValue)
    .Select(x => x.RoomId!.Value)
    .Distinct()
    .ToList();

        var bedIds = reservation.Items
            .Where(x => x.BedId.HasValue)
            .Select(x => x.BedId!.Value)
            .Distinct()
            .ToList();

        if (roomIds.Count > 0)
        {
            var rooms = await _context.Rooms.Where(x => roomIds.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var room in rooms)
                room.Status = HotelBookingSystem.Domain.EnumsNew.RoomStatus.Dirty;
        }

        if (bedIds.Count > 0)
        {
            var beds = await _context.Beds.Where(x => bedIds.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var bed in beds)
                bed.Status = HotelBookingSystem.Domain.EnumsNew.BedStatus.Cleaning;
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            reservation.Notes = string.IsNullOrWhiteSpace(reservation.Notes)
                ? request.Notes
                : reservation.Notes + " | CheckOut: " + request.Notes;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ReservationStatusResultDto
        {
            ReservationId = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            Status = reservation.Status,
            TotalAmount = reservation.TotalAmount,
            PaidAmount = reservation.PaidAmount,
            DueAmount = reservation.TotalAmount - reservation.PaidAmount
        };
    }
}