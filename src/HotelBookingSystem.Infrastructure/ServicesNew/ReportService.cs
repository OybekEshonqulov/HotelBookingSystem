using HotelBookingSystem.Application.DTOsNew.ReportNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReportService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var reservations = await _context.Reservations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PropertyId == propertyId)
            .ToListAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TotalReservations = reservations.Count,
            ConfirmedReservations = reservations.Count(x => x.Status == ReservationStatus.Confirmed),
            CheckedInReservations = reservations.Count(x => x.Status == ReservationStatus.CheckedIn),
            CheckedOutReservations = reservations.Count(x => x.Status == ReservationStatus.CheckedOut),
            CancelledReservations = reservations.Count(x => x.Status == ReservationStatus.Cancelled),
            TotalRevenue = reservations.Sum(x => x.TotalAmount),
            TotalPaid = reservations.Sum(x => x.PaidAmount),
            TotalDue = reservations.Sum(x => x.TotalAmount - x.PaidAmount)
        };
    }

    public async Task<OccupancyReportDto> GetOccupancyAsync(Guid propertyId, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var checkInUtc = DateTime.SpecifyKind(checkInDate, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(checkOutDate, DateTimeKind.Utc);

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var totalRooms = await _context.Rooms.CountAsync(x =>
            x.TenantId == tenantId && x.PropertyId == propertyId, cancellationToken);

        var reservedRooms = await _context.ReservationItems
            .Where(x => x.RoomId.HasValue)
            .Where(x => x.Room!.TenantId == tenantId && x.Room.PropertyId == propertyId)
            .Where(x => activeStatuses.Contains(x.Reservation.Status))
            .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
            .Select(x => x.RoomId!.Value)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalBeds = await _context.Beds.CountAsync(x =>
            x.TenantId == tenantId && x.Room.PropertyId == propertyId, cancellationToken);

        var reservedBeds = await _context.ReservationItems
            .Where(x => x.BedId.HasValue)
            .Where(x => x.Bed!.TenantId == tenantId && x.Bed.Room.PropertyId == propertyId)
            .Where(x => activeStatuses.Contains(x.Reservation.Status))
            .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
            .Select(x => x.BedId!.Value)
            .Distinct()
            .CountAsync(cancellationToken);

        return new OccupancyReportDto
        {
            PropertyId = propertyId,
            TotalRooms = totalRooms,
            ReservedRooms = reservedRooms,
            AvailableRooms = totalRooms - reservedRooms,
            TotalBeds = totalBeds,
            ReservedBeds = reservedBeds,
            AvailableBeds = totalBeds - reservedBeds
        };
    }
}