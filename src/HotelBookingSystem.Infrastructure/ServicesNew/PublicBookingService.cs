using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class PublicBookingService : IPublicBookingService
{
    private readonly AppDbContext _context;

    public PublicBookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PublicReservationResultDto> CreateReservationAsync(
        PublicCreateReservationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate.Date, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate.Date, DateTimeKind.Utc);

        if (checkOutUtc <= checkInUtc)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        if (request.Items.Count == 0)
            throw new BadRequestException("Kamida bitta reservation item bo‘lishi kerak.");

        var property = await _context.Properties
            .Include(x => x.Tenant)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);

        if (property is null || property.Tenant.Status != PropertyStatus.Active)
            throw new NotFoundException("Property topilmadi.");

        var tenantId = property.TenantId;
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
            throw new BadRequestException("Hozircha bitta reservation ichida room va bed aralash bo‘lishi mumkin emas.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var roomPrices = new Dictionary<Guid, decimal>();
        var bedPrices = new Dictionary<Guid, decimal>();

        if (roomIds.Count > 0)
        {
            var rooms = await _context.Rooms
                .AsNoTracking()
                .Include(x => x.RoomType)
                .Where(x => roomIds.Contains(x.Id) && x.PropertyId == request.PropertyId && x.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            if (rooms.Count != roomIds.Count)
                throw new NotFoundException("Roomlardan biri topilmadi.");

            if (rooms.Any(x => x.Status != RoomStatus.Available))
                throw new BadRequestException("Tanlangan roomlardan biri hozir band yoki bloklangan.");

            var reservedRoomIds = await _context.ReservationItems
                .Where(x => x.RoomId.HasValue && roomIds.Contains(x.RoomId.Value))
                .Where(x => activeStatuses.Contains(x.Reservation.Status))
                .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
                .Select(x => x.RoomId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (reservedRoomIds.Count > 0)
                throw new BadRequestException("Tanlangan roomlardan biri band.");

            roomPrices = rooms.ToDictionary(x => x.Id, x => x.RoomType.BasePrice);
        }

        if (bedIds.Count > 0)
        {
            var beds = await _context.Beds
                .AsNoTracking()
                .Include(x => x.Room)
                .ThenInclude(r => r.RoomType)
                .Where(x => bedIds.Contains(x.Id) && x.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            if (beds.Count != bedIds.Count)
                throw new NotFoundException("Bedlardan biri topilmadi.");

            if (beds.Any(x => x.Room.PropertyId != request.PropertyId))
                throw new BadRequestException("Bed noto‘g‘ri propertyga tegishli.");

            if (beds.Any(x => x.Status != BedStatus.Available))
                throw new BadRequestException("Tanlangan bedlardan biri hozir band yoki bloklangan.");

            var reservedBedIds = await _context.ReservationItems
                .Where(x => x.BedId.HasValue && bedIds.Contains(x.BedId.Value))
                .Where(x => activeStatuses.Contains(x.Reservation.Status))
                .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
                .Select(x => x.BedId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (reservedBedIds.Count > 0)
                throw new BadRequestException("Tanlangan bedlardan biri band.");

            bedPrices = beds.ToDictionary(x => x.Id, x => x.BedPrice ?? x.Room.RoomType.BasePrice);
        }

        var guest = await _context.Guests.FirstOrDefaultAsync(x =>
            x.TenantId == tenantId &&
            x.FirstName == request.GuestFirstName.Trim() &&
            x.LastName == request.GuestLastName.Trim() &&
            x.PhoneNumber == request.GuestPhoneNumber,
            cancellationToken);

        if (guest is null)
        {
            guest = new Guest
            {
                TenantId = tenantId,
                FirstName = request.GuestFirstName.Trim(),
                LastName = request.GuestLastName.Trim(),
                PhoneNumber = request.GuestPhoneNumber?.Trim(),
                Email = request.GuestEmail?.Trim(),
                PassportNumber = request.PassportNumber?.Trim(),
                Nationality = request.Nationality?.Trim()
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (guest.IsBlacklisted)
            throw new BadRequestException("Bu guest uchun bron qilish mumkin emas.");

        var reservation = new Reservation
        {
            TenantId = tenantId,
            PropertyId = request.PropertyId,
            GuestId = guest.Id,
            ReservationNumber = "RSV-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
            CheckInDate = checkInUtc,
            CheckOutDate = checkOutUtc,
            Status = ReservationStatus.Pending,
            Source = ReservationSource.Website,
            AdultsCount = request.AdultsCount,
            ChildrenCount = request.ChildrenCount,
            CurrencyCode = property.Tenant.CurrencyCode,
            PaidAmount = 0,
            Notes = request.Notes
        };

        var reservationItems = request.Items.Select(x =>
        {
            decimal unitPrice = 0;

            if (x.RoomId.HasValue)
                unitPrice = roomPrices[x.RoomId.Value];
            else if (x.BedId.HasValue)
                unitPrice = bedPrices[x.BedId.Value];

            return new ReservationItem
            {
                TenantId = tenantId,
                RoomId = x.RoomId,
                BedId = x.BedId,
                UnitPrice = unitPrice,
                Nights = nights,
                TotalPrice = unitPrice * nights
            };
        }).ToList();

        reservation.TotalAmount = reservationItems.Sum(x => x.TotalPrice);
        reservation.Items = reservationItems;

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return new PublicReservationResultDto
        {
            ReservationId = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            Status = reservation.Status,
            TotalAmount = reservation.TotalAmount,
            CurrencyCode = reservation.CurrencyCode,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            GuestFullName = guest.FirstName + " " + guest.LastName
        };
    }
}