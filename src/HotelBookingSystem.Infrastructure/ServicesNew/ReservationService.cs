using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.ReservationActionNew;
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
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate.Date, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate.Date, DateTimeKind.Utc);

        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        if (checkOutUtc <= checkInUtc)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        if (request.Items.Count == 0)
            throw new BadRequestException("Kamida bitta reservation item bo‘lishi kerak.");

        var tenantId = _currentUserService.TenantId.Value;

        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId && x.TenantId == tenantId, cancellationToken);

        if (property is null)
            throw new NotFoundException("Property topilmadi.");

        var nights = (checkOutUtc.Date - checkInUtc.Date).Days;
        if (nights <= 0)
            throw new BadRequestException("Nights 0 bo‘lishi mumkin emas.");

        var roomIds = request.Items.Where(x => x.RoomId.HasValue).Select(x => x.RoomId!.Value).Distinct().ToList();
        var bedIds = request.Items.Where(x => x.BedId.HasValue).Select(x => x.BedId!.Value).Distinct().ToList();

        foreach (var item in request.Items)
        {
            var hasRoom = item.RoomId.HasValue;
            var hasBed = item.BedId.HasValue;

            if (hasRoom == hasBed)
                throw new BadRequestException("Har bir item faqat RoomId yoki faqat BedId ga ega bo‘lishi kerak.");
        }

        if (roomIds.Count > 0 && bedIds.Count > 0)
            throw new BadRequestException("Bitta reservation ichida hozircha room va bed aralash bo‘lishi mumkin emas.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        if (roomIds.Count > 0)
        {
            var rooms = await _context.Rooms
                .AsNoTracking()
                .Where(x => roomIds.Contains(x.Id) && x.TenantId == tenantId && x.PropertyId == request.PropertyId)
                .ToListAsync(cancellationToken);

            if (rooms.Count != roomIds.Count)
                throw new Exception("Roomlardan biri topilmadi.");

            var reservedRoomIds = await _context.ReservationItems
                .Where(x => x.RoomId.HasValue && roomIds.Contains(x.RoomId.Value))
                .Where(x => activeStatuses.Contains(x.Reservation.Status))
                .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
                .Select(x => x.RoomId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (reservedRoomIds.Count > 0)
                throw new Exception("Tanlangan roomlardan biri band.");
        }

        if (bedIds.Count > 0)
        {
            var beds = await _context.Beds
                .AsNoTracking()
                .Where(x => bedIds.Contains(x.Id) && x.TenantId == tenantId)
                .Include(x => x.Room)
                .ToListAsync(cancellationToken);

            if (beds.Count != bedIds.Count)
                throw new Exception("Bedlardan biri topilmadi.");

            if (beds.Any(x => x.Room.PropertyId != request.PropertyId))
                throw new Exception("Bed noto‘g‘ri propertyga tegishli.");

            var reservedBedIds = await _context.ReservationItems
                .Where(x => x.BedId.HasValue && bedIds.Contains(x.BedId.Value))
                .Where(x => activeStatuses.Contains(x.Reservation.Status))
                    .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
                .Select(x => x.BedId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (reservedBedIds.Count > 0)
                throw new Exception("Tanlangan bedlardan biri band.");
        }

        var guest = new Guest
        {
            TenantId = tenantId,
            FirstName = request.GuestFirstName,
            LastName = request.GuestLastName,
            PhoneNumber = request.GuestPhoneNumber,
            Email = request.GuestEmail,
            PassportNumber = request.PassportNumber,
            Nationality = request.Nationality
        };

        await _context.Guests.AddAsync(guest, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var reservationNumber = $"RSV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";

        var reservation = new Reservation
        {
            TenantId = tenantId,
            ReservationNumber = reservationNumber,
            PropertyId = request.PropertyId,
            GuestId = guest.Id,
            CheckInDate = checkInUtc,
            CheckOutDate = checkOutUtc,
            Status = ReservationStatus.Confirmed,
            Source = request.Source,
            AdultsCount = request.AdultsCount,
            ChildrenCount = request.ChildrenCount,
            CurrencyCode = "UZS",
            PaidAmount = 0,
            Notes = request.Notes
        };

        var reservationItems = request.Items.Select(x => new ReservationItem
        {
            TenantId = tenantId,
            RoomId = x.RoomId,
            BedId = x.BedId,
            UnitPrice = x.UnitPrice,
            Nights = nights,
            TotalPrice = x.UnitPrice * nights
        }).ToList();

        reservation.TotalAmount = reservationItems.Sum(x => x.TotalPrice);
        reservation.Items = reservationItems;

        await _context.Reservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(reservation.Id, cancellationToken)
               ?? throw new Exception("Reservation yaratildi, lekin qayta o‘qib bo‘lmadi.");
    }

    public async Task<ReservationDto> CancelAsync(CancelReservationRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new Exception("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.ReservationId && x.TenantId == tenantId, cancellationToken);

        if (reservation is null)
            throw new Exception("Reservation topilmadi.");

        if (reservation.Status == ReservationStatus.CheckedOut)
            throw new Exception("Checked-out reservation bekor qilinmaydi.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new Exception("Reservation allaqachon bekor qilingan.");

        reservation.Status = ReservationStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            reservation.Notes = string.IsNullOrWhiteSpace(reservation.Notes)
                ? "Cancelled: " + request.Reason
                : reservation.Notes + " | Cancelled: " + request.Reason;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(reservation.Id, cancellationToken)
               ?? throw new Exception("Reservation bekor qilindi, lekin qayta o‘qib bo‘lmadi.");
    }

    public async Task<PagedResultDto<ReservationDto>> GetPagedAsync(ReservationFilterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var query = _context.Reservations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PropertyId == request.PropertyId)
            .Include(x => x.Guest)
            .Include(x => x.Items)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.GuestName))
        {
            var guestName = request.GuestName.Trim().ToLower();
            query = query.Where(x =>
                (x.Guest.FirstName + " " + x.Guest.LastName).ToLower().Contains(guestName));
        }

        if (request.CheckInFrom.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(request.CheckInFrom.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CheckInDate >= fromUtc);
        }

        if (request.CheckInTo.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(request.CheckInTo.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CheckInDate <= toUtc);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                ReservationNumber = x.ReservationNumber,
                PropertyId = x.PropertyId,
                GuestId = x.GuestId,
                GuestFullName = x.Guest.FirstName + " " + x.Guest.LastName,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                Status = x.Status,
                Source = x.Source,
                AdultsCount = x.AdultsCount,
                ChildrenCount = x.ChildrenCount,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                CurrencyCode = x.CurrencyCode,
                Notes = x.Notes,
                Items = x.Items.Select(i => new ReservationItemDto
                {
                    Id = i.Id,
                    RoomId = i.RoomId,
                    BedId = i.BedId,
                    UnitPrice = i.UnitPrice,
                    Nights = i.Nights,
                    TotalPrice = i.TotalPrice
                }).ToList()
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
        if (!_currentUserService.TenantId.HasValue)
            throw new Exception("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.Reservations
            .AsNoTracking()
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .Include(x => x.Guest)
            .Include(x => x.Items)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                ReservationNumber = x.ReservationNumber,
                PropertyId = x.PropertyId,
                GuestId = x.GuestId,
                GuestFullName = x.Guest.FirstName + " " + x.Guest.LastName,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                Status = x.Status,
                Source = x.Source,
                AdultsCount = x.AdultsCount,
                ChildrenCount = x.ChildrenCount,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                CurrencyCode = x.CurrencyCode,
                Notes = x.Notes,
                Items = x.Items.Select(i => new ReservationItemDto
                {
                    Id = i.Id,
                    RoomId = i.RoomId,
                    BedId = i.BedId,
                    UnitPrice = i.UnitPrice,
                    Nights = i.Nights,
                    TotalPrice = i.TotalPrice
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}