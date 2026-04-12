using HotelBookingSystem.Application.DTOsNew.AvailabilityNew;
using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PublicNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class PublicCatalogService : IPublicCatalogService
{
    private readonly AppDbContext _context;

    public PublicCatalogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<PublicPropertyCardDto>> GetPropertiesAsync(
        PublicPropertyFilterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 12 : request.PageSize;

        var query = _context.Properties
     .AsNoTracking()
     .Where(x => x.IsPublished)
     .Where(x => x.Tenant.Status == PropertyStatus.Active)
     .Include(x => x.Tenant)
     .Include(x => x.RoomTypes)
     .Include(x => x.Images)
     .Include(x => x.Reviews)
     .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                (x.Description != null && x.Description.ToLower().Contains(search)) ||
                (x.City != null && x.City.ToLower().Contains(search)) ||
                (x.Country != null && x.Country.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();
            query = query.Where(x => x.City != null && x.City.ToLower() == city);
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.Trim().ToLower();
            query = query.Where(x => x.Country != null && x.Country.ToLower() == country);
        }

        if (request.Type.HasValue)
            query = query.Where(x => x.Tenant.Type == request.Type.Value);

        if (request.GuestsCount.HasValue && request.GuestsCount.Value > 0)
            query = query.Where(x => x.RoomTypes.Any(rt => rt.Capacity >= request.GuestsCount.Value));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PublicPropertyCardDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                Name = x.Name,
                Description = x.Description,
                Address = x.Address,
                City = x.City,
                Country = x.Country,
                Type = x.Tenant.Type,
                MainImageUrl = x.Images
                    .OrderByDescending(i => i.IsMain)
                    .Select(i => i.FilePath)
                    .FirstOrDefault(),
                MinPrice = x.RoomTypes.Any(rt => rt.IsPublished) ? x.RoomTypes.Where(rt => rt.IsPublished).Min(rt => rt.BasePrice) : 0,
                AvgRating = x.Reviews.Any() ? Math.Round((decimal)x.Reviews.Average(r => r.Rating), 1) : null,
                ReviewCount = x.Reviews.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<PublicPropertyCardDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items
        };
    }

    public async Task<PublicPropertyDetailsDto?> GetPropertyByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AsNoTracking()
            .Where(x => x.Id == propertyId && x.IsPublished && x.Tenant.Status == PropertyStatus.Active)
            .Include(x => x.Tenant)
            .Include(x => x.Images)
            .Include(x => x.Reviews)
                .ThenInclude(r => r.User)
            .Include(x => x.RoomTypes)
                .ThenInclude(rt => rt.Rooms)
                    .ThenInclude(r => r.Beds)
            .Select(x => new PublicPropertyDetailsDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                Name = x.Name,
                Description = x.Description,
                Address = x.Address,
                City = x.City,
                Country = x.Country,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Type = x.Tenant.Type,
                PhoneNumber = x.Tenant.PhoneNumber,
                Email = x.Tenant.Email,
                AvgRating = x.Reviews.Any()
    ? decimal.Round((decimal)x.Reviews.Average(r => r.Rating), 1)
    : null,
                ReviewCount = x.Reviews.Count,
                Images = x.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.FileName)
                    .Select(i => new PublicPropertyImageDto
                    {
                        Id = i.Id,
                        FileName = i.FileName,
                        FilePath = i.FilePath,
                        IsMain = i.IsMain
                    })
                    .ToList(),
                Reviews = x.Reviews
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .Take(10)
                    .Select(r => new PublicPropertyReviewDto
                    {
                        Id = r.Id,
                        UserFullName = r.User.FirstName + " " + r.User.LastName,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAtUtc = r.CreatedAtUtc
                    })
                    .ToList(),
                RoomTypes = x.RoomTypes
    .Where(rt => rt.IsPublished)
    .OrderBy(rt => rt.BasePrice)
    .Select(rt => new PublicRoomTypeDto
    {
        Id = rt.Id,
        Name = rt.Name,
        Capacity = rt.Capacity,
        BasePrice = rt.BasePrice,
        RoomsCount = rt.Rooms.Count(r => r.IsPublished),
        BedsCount = rt.Rooms.SelectMany(r => r.Beds).Count(b => b.IsPublished)
    })
    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AvailableRoomDto>> GetAvailableRoomsAsync(
        PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate.Date, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate.Date, DateTimeKind.Utc);

        if (checkOutUtc <= checkInUtc)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        var property = await _context.Properties
            .AsNoTracking()
            .Include(x => x.Tenant)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);

        if (property is null || property.Tenant.Status != PropertyStatus.Active || !property.IsPublished)
            throw new NotFoundException("Property topilmadi.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var reservedRoomIds = await _context.ReservationItems
            .Where(x => x.RoomId.HasValue)
            .Where(x => x.Room!.PropertyId == request.PropertyId)
            .Where(x => activeStatuses.Contains(x.Reservation.Status))
            .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
            .Select(x => x.RoomId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _context.Rooms
            .AsNoTracking()
            .Where(x => x.PropertyId == request.PropertyId)
            .Where(x => x.IsPublished)
            .Where(x => x.RoomType.IsPublished)
            .Where(x => x.Status == RoomStatus.Available)
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

    public async Task<List<AvailableBedDto>> GetAvailableBedsAsync(
        PublicAvailabilitySearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var checkInUtc = DateTime.SpecifyKind(request.CheckInDate.Date, DateTimeKind.Utc);
        var checkOutUtc = DateTime.SpecifyKind(request.CheckOutDate.Date, DateTimeKind.Utc);

        if (checkOutUtc <= checkInUtc)
            throw new BadRequestException("Check-out sana check-in sanadan katta bo‘lishi kerak.");

        var property = await _context.Properties
            .AsNoTracking()
            .Include(x => x.Tenant)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);

        if (property is null || property.Tenant.Status != PropertyStatus.Active || !property.IsPublished)
            throw new NotFoundException("Property topilmadi.");

        var activeStatuses = new[]
        {
            ReservationStatus.Pending,
            ReservationStatus.Confirmed,
            ReservationStatus.CheckedIn
        };

        var reservedBedIds = await _context.ReservationItems
            .Where(x => x.BedId.HasValue)
            .Where(x => x.Bed!.Room.PropertyId == request.PropertyId)
            .Where(x => activeStatuses.Contains(x.Reservation.Status))
            .Where(x => checkInUtc < x.Reservation.CheckOutDate && checkOutUtc > x.Reservation.CheckInDate)
            .Select(x => x.BedId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _context.Beds
            .AsNoTracking()
            .Where(x => x.Room.PropertyId == request.PropertyId)
            .Where(x => x.Status == BedStatus.Available)
            .Where(x => x.IsPublished)
            .Where(x => x.Room.IsPublished)
            .Where(x => x.Room.RoomType.IsPublished)
            .Where(x => !reservedBedIds.Contains(x.Id))
            .Include(x => x.Room)
                .ThenInclude(r => r.RoomType)
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