using HotelBookingSystem.Application.DTOsNew.TenantNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class TenantService : ITenantService
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;

    public TenantService(
        AppDbContext context,
        IPasswordService passwordService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordService = passwordService;
        _currentUserService = currentUserService;
    }

    public async Task<TenantDto> CreateAsync(CreateTenantRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureTenantIsUniqueAsync(request.Name, request.Subdomain, cancellationToken);

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Type = request.Type,
            Status = PropertyStatus.Active,
            Subdomain = request.Subdomain?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Email = request.Email?.Trim(),
            Address = request.Address?.Trim(),
            CurrencyCode = request.CurrencyCode.Trim(),
            TimeZone = request.TimeZone.Trim()
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return MapTenant(tenant);
    }

    public async Task<TenantWithOwnerResultDto> CreateWithOwnerAsync(
        CreateTenantWithOwnerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Tenant nomi kiritilishi shart.");

        if (string.IsNullOrWhiteSpace(request.OwnerFirstName))
            throw new BadRequestException("Owner ismi kiritilishi shart.");

        if (string.IsNullOrWhiteSpace(request.OwnerLastName))
            throw new BadRequestException("Owner familiyasi kiritilishi shart.");

        if (string.IsNullOrWhiteSpace(request.OwnerEmail))
            throw new BadRequestException("Owner email kiritilishi shart.");

        if (string.IsNullOrWhiteSpace(request.OwnerPassword))
            throw new BadRequestException("Owner paroli kiritilishi shart.");

        await EnsureTenantIsUniqueAsync(request.Name, request.Subdomain, cancellationToken);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Type = request.Type,
            Status = PropertyStatus.Active,
            Subdomain = request.Subdomain?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Email = request.Email?.Trim(),
            Address = request.Address?.Trim(),
            CurrencyCode = request.CurrencyCode.Trim(),
            TimeZone = request.TimeZone.Trim()
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        var ownerRole = new AppRole
        {
            TenantId = tenant.Id,
            Name = "Owner",
            Description = "Tenant owner full access inside own tenant"
        };

        _context.AppRoles.Add(ownerRole);
        await _context.SaveChangesAsync(cancellationToken);

        var ownerPermissionCodes = GetOwnerPermissionCodes();

        var permissions = await _context.Permissions
            .Where(x => ownerPermissionCodes.Contains(x.Code))
            .ToListAsync(cancellationToken);

        foreach (var permission in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = ownerRole.Id,
                PermissionId = permission.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var ownerUser = new AppUser
        {
            TenantId = tenant.Id,
            FirstName = request.OwnerFirstName.Trim(),
            LastName = request.OwnerLastName.Trim(),
            Email = request.OwnerEmail.Trim(),
            PasswordHash = _passwordService.HashPassword(request.OwnerPassword),
            IsActive = true
        };

        _context.AppUsers.Add(ownerUser);
        await _context.SaveChangesAsync(cancellationToken);

        _context.AppUserRoles.Add(new AppUserRole
        {
            UserId = ownerUser.Id,
            RoleId = ownerRole.Id
        });

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new TenantWithOwnerResultDto
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            TenantSubdomain = tenant.Subdomain ?? string.Empty,
            OwnerUserId = ownerUser.Id,
            OwnerFullName = $"{ownerUser.FirstName} {ownerUser.LastName}",
            OwnerEmail = ownerUser.Email,
            OwnerRoleId = ownerRole.Id,
            OwnerRoleName = ownerRole.Name
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

    public async Task<TenantDto?> GetMyTenantAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            return null;

        return await _context.Tenants
            .AsNoTracking()
            .Where(x => x.Id == _currentUserService.TenantId.Value)
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
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task EnsureTenantIsUniqueAsync(
        string name,
        string? subdomain,
        CancellationToken cancellationToken)
    {
        var exists = await _context.Tenants.AnyAsync(x =>
            x.Name == name.Trim() ||
            (!string.IsNullOrWhiteSpace(subdomain) && x.Subdomain == subdomain.Trim()),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bunday tenant allaqachon mavjud.");
    }

    private static TenantDto MapTenant(Tenant tenant)
    {
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

    private static List<string> GetOwnerPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.UsersView,
            PermissionCodes.UsersCreate,
            PermissionCodes.UsersEdit,

            PermissionCodes.RolesView,
            PermissionCodes.RolesCreate,
            PermissionCodes.RolesEdit,

            PermissionCodes.PropertiesView,
            PermissionCodes.PropertiesCreate,
            PermissionCodes.PropertiesEdit,

            PermissionCodes.ReservationsView,
            PermissionCodes.ReservationsCreate,
            PermissionCodes.ReservationsEdit,
            PermissionCodes.ReservationsCancel,

            PermissionCodes.PaymentsView,
            PermissionCodes.PaymentsCreate,

            PermissionCodes.ReportsView
        };
    }
}