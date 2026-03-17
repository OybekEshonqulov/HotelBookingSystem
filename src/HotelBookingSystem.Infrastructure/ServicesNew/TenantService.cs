using HotelBookingSystem.Application.DTOsNew.TenantNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class TenantService : ITenantService
{
    private readonly AppDbContext _context;

    public TenantService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto> CreateAsync(CreateTenantRequestDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Tenants.AnyAsync(x =>
            x.Name == request.Name ||
            (!string.IsNullOrWhiteSpace(request.Subdomain) && x.Subdomain == request.Subdomain),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bunday tenant allaqachon mavjud.");

        var tenant = new Tenant
        {
            Name = request.Name,
            Type = request.Type,
            Status = PropertyStatus.Active,
            Subdomain = request.Subdomain,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            CurrencyCode = request.CurrencyCode,
            TimeZone = request.TimeZone
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Type = tenant.Type,
            Subdomain = tenant.Subdomain,
            PhoneNumber = tenant.PhoneNumber,
            Email = tenant.Email,
            Address = tenant.Address,
            CurrencyCode = tenant.CurrencyCode,
            TimeZone = tenant.TimeZone
        };
    }

    public async Task<List<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TenantDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Subdomain = x.Subdomain,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Address = x.Address,
                CurrencyCode = x.CurrencyCode,
                TimeZone = x.TimeZone
            })
            .ToListAsync(cancellationToken);
    }
}