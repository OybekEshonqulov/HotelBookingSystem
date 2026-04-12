using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PaymentNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PaymentService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.ReservationId, cancellationToken);

        if (reservation is null || !CanAccessTenant(reservation.TenantId))
            throw new NotFoundException("Reservation topilmadi.");

        var payment = new Payment
        {
            TenantId = reservation.TenantId,
            ReservationId = request.ReservationId,
            Amount = request.Amount,
            Method = request.Method,
            TransactionId = request.TransactionId,
            PaidAtUtc = request.PaidAtUtc,
            Notes = request.Notes
        };

        _context.Payments.Add(payment);

        reservation.PaidAmount += request.Amount;

        await _context.SaveChangesAsync(cancellationToken);

        return await BuildPaymentDtoAsync(payment.Id, cancellationToken);
    }

    public async Task<PagedResultDto<PaymentDto>> GetPagedAsync(PaymentFilterRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Include(x => x.Reservation)
                .ThenInclude(r => r.Guest)
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

        if (request.ReservationId.HasValue)
            query = query.Where(x => x.ReservationId == request.ReservationId.Value);

        if (request.PaidFrom.HasValue)
            query = query.Where(x => x.PaidAtUtc >= request.PaidFrom.Value);

        if (request.PaidTo.HasValue)
            query = query.Where(x => x.PaidAtUtc <= request.PaidTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                (x.TransactionId != null && x.TransactionId.ToLower().Contains(search)) ||
                x.Reservation.ReservationNumber.ToLower().Contains(search) ||
                (x.Reservation.Guest.FirstName + " " + x.Reservation.Guest.LastName).ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.PaidAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PaymentDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                ReservationId = x.ReservationId,
                ReservationNumber = x.Reservation.ReservationNumber,
                GuestFullName = x.Reservation.Guest.FirstName + " " + x.Reservation.Guest.LastName,
                Amount = x.Amount,
                Method = x.Method,
                TransactionId = x.TransactionId,
                PaidAtUtc = x.PaidAtUtc,
                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<PaymentDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items
        };
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

    private async Task<PaymentDto> BuildPaymentDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new PaymentDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                ReservationId = x.ReservationId,
                ReservationNumber = x.Reservation.ReservationNumber,
                GuestFullName = x.Reservation.Guest.FirstName + " " + x.Reservation.Guest.LastName,
                Amount = x.Amount,
                Method = x.Method,
                TransactionId = x.TransactionId,
                PaidAtUtc = x.PaidAtUtc,
                Notes = x.Notes
            })
            .FirstAsync(cancellationToken);
    }
}