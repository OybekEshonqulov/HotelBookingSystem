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
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var exists = await _context.Properties.AnyAsync(x =>
            x.TenantId == _currentUserService.TenantId.Value &&
            x.Name == request.Name,
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi property allaqachon mavjud.");

        var property = new Property
        {
            TenantId = _currentUserService.TenantId.Value,
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

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
            Longitude = property.Longitude
        };
    }

    public async Task<List<PropertyDto>> GetMyPropertiesAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return await _context.Properties
            .AsNoTracking()
            .Where(x => x.TenantId == _currentUserService.TenantId.Value)
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
                Longitude = x.Longitude
            })
            .ToListAsync(cancellationToken);
    }
}