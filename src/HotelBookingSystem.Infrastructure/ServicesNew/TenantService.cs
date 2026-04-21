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
        await EnsureTenantIsUniqueAsync(request.Name, request.Subdomain, null, cancellationToken);

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

        return await BuildTenantDtoAsync(tenant.Id, cancellationToken);
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

        await EnsureTenantIsUniqueAsync(request.Name, request.Subdomain, null, cancellationToken);

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

        var ownerRole = await CreateRoleAsync(
            tenant.Id,
            "Owner",
            "Tenant owner full access inside own tenant",
            GetOwnerPermissionCodes(),
            cancellationToken);

        await CreateRoleAsync(
            tenant.Id,
            "Manager",
            "Hotel manager",
            GetManagerPermissionCodes(),
            cancellationToken);

        await CreateRoleAsync(
            tenant.Id,
            "Reception",
            "Front desk / reservations",
            GetReceptionPermissionCodes(),
            cancellationToken);

        await CreateRoleAsync(
            tenant.Id,
            "Cashier",
            "Payments only",
            GetCashierPermissionCodes(),
            cancellationToken);

        await CreateRoleAsync(
            tenant.Id,
            "Housekeeping",
            "Housekeeping operations",
            GetHousekeepingPermissionCodes(),
            cancellationToken);

        await CreateRoleAsync(
            tenant.Id,
            "ReportViewer",
            "Reports read only",
            GetReportViewerPermissionCodes(),
            cancellationToken);

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
                Status = x.Status,
                Subdomain = x.Subdomain,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Address = x.Address,
                CurrencyCode = x.CurrencyCode,
                TimeZone = x.TimeZone,
                TotalUsers = x.Users.Count(),
                TotalProperties = x.Properties.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TenantDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Status = x.Status,
                Subdomain = x.Subdomain,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Address = x.Address,
                CurrencyCode = x.CurrencyCode,
                TimeZone = x.TimeZone,
                TotalUsers = x.Users.Count(),
                TotalProperties = x.Properties.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TenantDto?> GetMyTenantAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            return null;

        return await GetByIdAsync(_currentUserService.TenantId.Value, cancellationToken);
    }

    public async Task<TenantDto> UpdateAsync(Guid id, UpdateTenantRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant topilmadi.");

        await EnsureTenantIsUniqueAsync(request.Name, request.Subdomain, id, cancellationToken);

        tenant.Name = request.Name.Trim();
        tenant.Type = request.Type;
        tenant.Subdomain = request.Subdomain?.Trim();
        tenant.PhoneNumber = request.PhoneNumber?.Trim();
        tenant.Email = request.Email?.Trim();
        tenant.Address = request.Address?.Trim();
        tenant.CurrencyCode = request.CurrencyCode.Trim();
        tenant.TimeZone = request.TimeZone.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return await BuildTenantDtoAsync(tenant.Id, cancellationToken);
    }

    public async Task<TenantDto> UpdateStatusAsync(Guid id, UpdateTenantStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant topilmadi.");

        tenant.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return await BuildTenantDtoAsync(tenant.Id, cancellationToken);
    }

    private async Task<TenantDto> BuildTenantDtoAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenant = await GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant topilmadi.");

        return tenant;
    }

    private async Task EnsureTenantIsUniqueAsync(
        string name,
        string? subdomain,
        Guid? excludeTenantId,
        CancellationToken cancellationToken)
    {
        var trimmedName = name.Trim();
        var trimmedSubdomain = subdomain?.Trim();

        var exists = await _context.Tenants.AnyAsync(x =>
            x.Id != excludeTenantId &&
            (
                x.Name == trimmedName ||
                (!string.IsNullOrWhiteSpace(trimmedSubdomain) && x.Subdomain == trimmedSubdomain)
            ),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bunday tenant allaqachon mavjud.");
    }

    private async Task<AppRole> CreateRoleAsync(
        Guid tenantId,
        string roleName,
        string description,
        List<string> permissionCodes,
        CancellationToken cancellationToken)
    {
        var role = new AppRole
        {
            TenantId = tenantId,
            Name = roleName,
            Description = description
        };

        _context.AppRoles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        var permissions = await _context.Permissions
            .Where(x => permissionCodes.Contains(x.Code))
            .ToListAsync(cancellationToken);

        foreach (var permission in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return role;
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

    private static List<string> GetManagerPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.UsersView,
            PermissionCodes.RolesView,

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

    private static List<string> GetReceptionPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.PropertiesView,
            PermissionCodes.ReservationsView,
            PermissionCodes.ReservationsCreate,
            PermissionCodes.ReservationsEdit,
            PermissionCodes.ReservationsCancel,
            PermissionCodes.PaymentsView,
            PermissionCodes.PaymentsCreate
        };
    }

    private static List<string> GetCashierPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.PaymentsView,
            PermissionCodes.PaymentsCreate,
            PermissionCodes.ReservationsView
        };
    }

    private static List<string> GetHousekeepingPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.PropertiesView,
            PermissionCodes.ReservationsView
        };
    }

    private static List<string> GetReportViewerPermissionCodes()
    {
        return new List<string>
        {
            PermissionCodes.ReportsView,
            PermissionCodes.PropertiesView,
            PermissionCodes.ReservationsView,
            PermissionCodes.PaymentsView
        };
    }
}