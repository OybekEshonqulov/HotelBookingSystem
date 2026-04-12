using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.ReservationNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReservationService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenantId = ResolveTenantId(request.TenantId);
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId && x.TenantId == tenantId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        var guest = await _context.Guests
            .FirstOrDefaultAsync(x => x.Id == request.GuestId && x.TenantId == tenantId, cancellationToken);

        if (guest is null)
            throw new NotFoundException("Guest topilmadi.");

        var reservation = new Reservation
        {
            TenantId = tenantId,
            PropertyId = request.PropertyId,
            GuestId = request.GuestId,
            ReservationNumber = "RSV-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = ReservationStatus.Pending,
            Source = request.Source,
            AdultsCount = request.AdultsCount,
            ChildrenCount = request.ChildrenCount,
            TotalAmount = request.TotalAmount,
            PaidAmount = 0,
            CurrencyCode = request.CurrencyCode,
            Notes = request.Notes
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return await BuildReservationDtoAsync(reservation.Id, cancellationToken);
    }

    public async Task<PagedResultDto<ReservationDto>> GetPagedAsync(ReservationFilterRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = _context.Reservations
            .AsNoTracking()
            .Include(x => x.Guest)
            .Include(x => x.Property)
            .AsQueryable();

        if (_currentUserService.IsSuperAdmin)
        {
            if (request.TenantId.HasValue)
                query = query.Where(x => x.TenantId == request.TenantId.Value);
        }
        else
        {
            var tenantId = GetCurrentTenantId();
            query = query.Where(x => x.TenantId == tenantId);
        }

        if (request.PropertyId.HasValue)
            query = query.Where(x => x.PropertyId == request.PropertyId.Value);

        if (request.GuestId.HasValue)
            query = query.Where(x => x.GuestId == request.GuestId.Value);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (request.CheckInFrom.HasValue)
            query = query.Where(x => x.CheckInDate >= request.CheckInFrom.Value);

        if (request.CheckInTo.HasValue)
            query = query.Where(x => x.CheckInDate <= request.CheckInTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.ReservationNumber.ToLower().Contains(search) ||
                (x.Guest.FirstName + " " + x.Guest.LastName).ToLower().Contains(search) ||
                x.Property.Name.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                PropertyName = x.Property.Name,
                GuestId = x.GuestId,
                GuestFullName = x.Guest.FirstName + " " + x.Guest.LastName,
                ReservationNumber = x.ReservationNumber,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                Status = x.Status,
                Source = x.Source,
                AdultsCount = x.AdultsCount,
                ChildrenCount = x.ChildrenCount,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                CurrencyCode = x.CurrencyCode,
                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ReservationDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items
        };
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (reservation is null || !CanAccessTenant(reservation.TenantId))
            return null;

        return await BuildReservationDtoAsync(id, cancellationToken);
    }

    public async Task<ReservationDto> ChangeStatusAsync(Guid id, ChangeReservationStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (reservation is null || !CanAccessTenant(reservation.TenantId))
            throw new NotFoundException("Reservation topilmadi.");

        reservation.Status = request.Status;
        reservation.Notes = string.IsNullOrWhiteSpace(request.Notes)
            ? reservation.Notes
            : request.Notes.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildReservationDtoAsync(reservation.Id, cancellationToken);
    }

    private Guid ResolveTenantId(Guid? requestedTenantId)
    {
        if (_currentUserService.IsSuperAdmin)
        {
            if (requestedTenantId.HasValue)
                return requestedTenantId.Value;

            throw new BadRequestException("SuperAdmin uchun TenantId yuborilishi shart.");
        }

        return GetCurrentTenantId();
    }

    private Guid GetCurrentTenantId()
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return _currentUserService.TenantId.Value;
    }

    private bool CanAccessTenant(Guid tenantId)
    {
        return _currentUserService.IsSuperAdmin ||
               (_currentUserService.TenantId.HasValue && _currentUserService.TenantId.Value == tenantId);
    }

    private async Task<ReservationDto> BuildReservationDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PropertyId = x.PropertyId,
                PropertyName = x.Property.Name,
                GuestId = x.GuestId,
                GuestFullName = x.Guest.FirstName + " " + x.Guest.LastName,
                ReservationNumber = x.ReservationNumber,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                Status = x.Status,
                Source = x.Source,
                AdultsCount = x.AdultsCount,
                ChildrenCount = x.ChildrenCount,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                CurrencyCode = x.CurrencyCode,
                Notes = x.Notes
            })
            .FirstAsync(cancellationToken);
    }
}