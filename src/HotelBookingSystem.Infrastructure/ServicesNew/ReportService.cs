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

    public async Task<TenantDashboardDto> GetTenantDashboardAsync(TenantDashboardRequestDto request, CancellationToken cancellationToken = default)
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