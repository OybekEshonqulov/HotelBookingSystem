using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.PropertyNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class PropertyService : IPropertyService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PropertyService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenantId = ResolveTenantId(request.TenantId);

        var exists = await _context.Properties.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.Name == request.Name.Trim(),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi property allaqachon mavjud.");

        var property = new Property
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Address = request.Address?.Trim(),
            City = request.City?.Trim(),
            Country = request.Country?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsPublished = false
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

        return Map(property);
    }

    public async Task<List<PropertyDto>> GetAccessiblePropertiesAsync(Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.AsNoTracking().AsQueryable();

        if (_currentUserService.IsSuperAdmin)
        {
            if (tenantId.HasValue)
                query = query.Where(x => x.TenantId == tenantId.Value);
        }
        else
        {
            var currentTenantId = GetCurrentTenantId();
            query = query.Where(x => x.TenantId == currentTenantId);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new PropertyDto
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
                IsPublished = x.IsPublished
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyDto> UpdatePublishStatusAsync(Guid id, UpdatePublishStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (property is null || !CanAccessTenant(property.TenantId))
            throw new NotFoundException("Property topilmadi.");

        property.IsPublished = request.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        return Map(property);
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

    private static PropertyDto Map(Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            TenantId = property.TenantId,
            Name = property.Name,
            Description = property.Description,
            Address = property.Address,
            City = property.City,
            Country = property.Country,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            IsPublished = property.IsPublished
        };
    }
}