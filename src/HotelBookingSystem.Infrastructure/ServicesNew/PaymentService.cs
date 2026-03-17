using HotelBookingSystem.Application.DTOsNew.PaymentNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
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
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        if (request.Amount <= 0)
            throw new BadRequestException("To‘lov summasi 0 dan katta bo‘lishi kerak.");

        var tenantId = _currentUserService.TenantId.Value;

        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.ReservationId && x.TenantId == tenantId, cancellationToken);

        if (reservation is null)
            throw new NotFoundException("Reservation topilmadi.");

        var payment = new Payment
        {
            TenantId = tenantId,
            ReservationId = request.ReservationId,
            Amount = request.Amount,
            Method = request.Method,
            Status = PaymentStatus.Paid,
            PaidAtUtc = DateTime.UtcNow,
            CurrencyCode = reservation.CurrencyCode,
            TransactionId = request.TransactionId,
            Notes = request.Notes
        };

        _context.Payments.Add(payment);

        reservation.PaidAmount += request.Amount;
        if (reservation.PaidAmount > reservation.TotalAmount)
            reservation.PaidAmount = reservation.TotalAmount;

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            ReservationId = payment.ReservationId,
            Amount = payment.Amount,
            Method = payment.Method,
            Status = payment.Status,
            PaidAtUtc = payment.PaidAtUtc,
            CurrencyCode = payment.CurrencyCode,
            TransactionId = payment.TransactionId,
            Notes = payment.Notes
        };
    }

    public async Task<List<PaymentDto>> GetByReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.Payments
            .AsNoTracking()
            .Where(x => x.ReservationId == reservationId && x.TenantId == tenantId)
            .OrderByDescending(x => x.PaidAtUtc)
            .Select(x => new PaymentDto
            {
                Id = x.Id,
                ReservationId = x.ReservationId,
                Amount = x.Amount,
                Method = x.Method,
                Status = x.Status,
                PaidAtUtc = x.PaidAtUtc,
                CurrencyCode = x.CurrencyCode,
                TransactionId = x.TransactionId,
                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);
    }
}