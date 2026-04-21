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

    public async Task<TenantDashboardDto> GetTenantDashboardAsync(
        TenantDashboardRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = ResolveTenantId(request.TenantId);

        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);

        if (tenant is null)
            throw new NotFoundException("Tenant topilmadi.");

        var fromUtc = request.FromUtc ?? DateTime.UtcNow.Date.AddDays(-30);
        var toUtc = request.ToUtc ?? DateTime.UtcNow;

        var reservationsQuery = _context.Reservations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.CreatedAtUtc >= fromUtc && x.CreatedAtUtc <= toUtc);

        var totalProperties = await _context.Properties.CountAsync(x => x.TenantId == tenantId, cancellationToken);
        var totalRooms = await _context.Rooms.CountAsync(x => x.TenantId == tenantId, cancellationToken);
        var totalBeds = await _context.Beds.CountAsync(x => x.TenantId == tenantId, cancellationToken);

        var totalReservations = await reservationsQuery.CountAsync(cancellationToken);
        var pendingReservations = await reservationsQuery.CountAsync(x => x.Status == ReservationStatus.Pending, cancellationToken);
        var confirmedReservations = await reservationsQuery.CountAsync(x => x.Status == ReservationStatus.Confirmed, cancellationToken);
        var checkedInReservations = await reservationsQuery.CountAsync(x => x.Status == ReservationStatus.CheckedIn, cancellationToken);
        var checkedOutReservations = await reservationsQuery.CountAsync(x => x.Status == ReservationStatus.CheckedOut, cancellationToken);
        var cancelledReservations = await reservationsQuery.CountAsync(x => x.Status == ReservationStatus.Cancelled, cancellationToken);

        var totalRevenue = await reservationsQuery.SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0;
        var totalPaid = await reservationsQuery.SumAsync(x => (decimal?)x.PaidAmount, cancellationToken) ?? 0;

        var occupiedCount = checkedInReservations + checkedOutReservations + confirmedReservations;
        var occupancyRate = totalReservations == 0
            ? 0
            : decimal.Round((decimal)occupiedCount / totalReservations * 100, 2);

        return new TenantDashboardDto
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            TotalProperties = totalProperties,
            TotalRooms = totalRooms,
            TotalBeds = totalBeds,
            TotalReservations = totalReservations,
            PendingReservations = pendingReservations,
            ConfirmedReservations = confirmedReservations,
            CheckedInReservations = checkedInReservations,
            CheckedOutReservations = checkedOutReservations,
            CancelledReservations = cancelledReservations,
            TotalRevenue = totalRevenue,
            TotalPaid = totalPaid,
            OutstandingAmount = totalRevenue - totalPaid,
            OccupancyRate = occupancyRate
        };
    }

    public async Task<SystemDashboardDto> GetSystemDashboardAsync(
        SystemDashboardRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsSuperAdmin)
            throw new ForbiddenException("Faqat SuperAdmin global dashboardni ko‘ra oladi.");

        var fromUtc = request.FromUtc ?? DateTime.UtcNow.Date.AddDays(-30);
        var toUtc = request.ToUtc ?? DateTime.UtcNow;

        var tenants = await _context.Tenants
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Type,
                x.Status,
                TotalUsers = x.Users.Count(),
                TotalProperties = x.Properties.Count()
            })
            .ToListAsync(cancellationToken);

        var tenantIds = tenants.Select(x => x.Id).ToList();

        var rooms = await _context.Rooms
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId))
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var beds = await _context.Beds
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId))
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var reservationStats = await _context.Reservations
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId))
            .Where(x => x.CreatedAtUtc >= fromUtc && x.CreatedAtUtc <= toUtc)
            .GroupBy(x => x.TenantId)
            .Select(g => new
            {
                TenantId = g.Key,
                TotalReservations = g.Count(),
                PendingReservations = g.Count(x => x.Status == ReservationStatus.Pending),
                ConfirmedReservations = g.Count(x => x.Status == ReservationStatus.Confirmed),
                CheckedInReservations = g.Count(x => x.Status == ReservationStatus.CheckedIn),
                CheckedOutReservations = g.Count(x => x.Status == ReservationStatus.CheckedOut),
                CancelledReservations = g.Count(x => x.Status == ReservationStatus.Cancelled),
                TotalRevenue = g.Sum(x => (decimal?)x.TotalAmount) ?? 0,
                TotalPaid = g.Sum(x => (decimal?)x.PaidAmount) ?? 0
            })
            .ToListAsync(cancellationToken);

        var roomsDict = rooms.ToDictionary(x => x.TenantId, x => x.Count);
        var bedsDict = beds.ToDictionary(x => x.TenantId, x => x.Count);
        var reservationDict = reservationStats.ToDictionary(x => x.TenantId, x => x);

        var tenantItems = tenants.Select(t =>
        {
            reservationDict.TryGetValue(t.Id, out var stat);

            var totalReservations = stat?.TotalReservations ?? 0;
            var confirmed = stat?.ConfirmedReservations ?? 0;
            var checkedIn = stat?.CheckedInReservations ?? 0;
            var checkedOut = stat?.CheckedOutReservations ?? 0;
            var occupiedCount = confirmed + checkedIn + checkedOut;

            return new SystemTenantSummaryDto
            {
                TenantId = t.Id,
                TenantName = t.Name,
                TenantType = t.Type,
                Status = t.Status,

                TotalUsers = t.TotalUsers,
                TotalProperties = t.TotalProperties,
                TotalRooms = roomsDict.GetValueOrDefault(t.Id, 0),
                TotalBeds = bedsDict.GetValueOrDefault(t.Id, 0),

                TotalReservations = totalReservations,
                PendingReservations = stat?.PendingReservations ?? 0,
                ConfirmedReservations = confirmed,
                CheckedInReservations = checkedIn,
                CheckedOutReservations = checkedOut,
                CancelledReservations = stat?.CancelledReservations ?? 0,

                TotalRevenue = stat?.TotalRevenue ?? 0,
                TotalPaid = stat?.TotalPaid ?? 0,
                OutstandingAmount = (stat?.TotalRevenue ?? 0) - (stat?.TotalPaid ?? 0),
                OccupancyRate = totalReservations == 0
                    ? 0
                    : decimal.Round((decimal)occupiedCount / totalReservations * 100, 2)
            };
        }).ToList();

        return new SystemDashboardDto
        {
            TotalTenants = tenantItems.Count,
            ActiveTenants = tenantItems.Count(x => x.Status == PropertyStatus.Active),
            InactiveTenants = tenantItems.Count(x => x.Status != PropertyStatus.Active),

            TotalUsers = tenantItems.Sum(x => x.TotalUsers),
            TotalProperties = tenantItems.Sum(x => x.TotalProperties),
            TotalRooms = tenantItems.Sum(x => x.TotalRooms),
            TotalBeds = tenantItems.Sum(x => x.TotalBeds),

            TotalReservations = tenantItems.Sum(x => x.TotalReservations),
            PendingReservations = tenantItems.Sum(x => x.PendingReservations),
            ConfirmedReservations = tenantItems.Sum(x => x.ConfirmedReservations),
            CheckedInReservations = tenantItems.Sum(x => x.CheckedInReservations),
            CheckedOutReservations = tenantItems.Sum(x => x.CheckedOutReservations),
            CancelledReservations = tenantItems.Sum(x => x.CancelledReservations),

            TotalRevenue = tenantItems.Sum(x => x.TotalRevenue),
            TotalPaid = tenantItems.Sum(x => x.TotalPaid),
            OutstandingAmount = tenantItems.Sum(x => x.OutstandingAmount),
            OccupancyRate = tenantItems.Sum(x => x.TotalReservations) == 0
                ? 0
                : decimal.Round(
                    (decimal)(tenantItems.Sum(x => x.ConfirmedReservations) + tenantItems.Sum(x => x.CheckedInReservations) + tenantItems.Sum(x => x.CheckedOutReservations))
                    / tenantItems.Sum(x => x.TotalReservations) * 100, 2),

            Tenants = tenantItems
        };
    }

    private Guid ResolveTenantId(Guid? requestedTenantId)
    {
        if (_currentUserService.IsSuperAdmin)
        {
            if (requestedTenantId.HasValue)
                return requestedTenantId.Value;

            throw new BadRequestException("SuperAdmin uchun TenantId yuborilishi shart.");
        }

        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return _currentUserService.TenantId.Value;
    }
}