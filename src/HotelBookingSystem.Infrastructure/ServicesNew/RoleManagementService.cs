using HotelBookingSystem.Application.DTOsNew.RoleManagementNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class RoleManagementService : IRoleManagementService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RoleManagementService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var exists = await _context.AppRoles.AnyAsync(x =>
            x.TenantId == tenantId && x.Name == request.Name, cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi role allaqachon mavjud.");

        var permissions = await _context.Permissions
            .Where(x => request.PermissionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var role = new AppRole
        {
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description
        };

        _context.AppRoles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var permission in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await _context.AppRoles
            .AsNoTracking()
            .Where(x => x.Id == role.Id)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.RolePermissions.Select(rp => rp.Permission.Code).ToList()
            })
            .FirstAsync(cancellationToken);
    }

    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        return await _context.AppRoles
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.RolePermissions.Select(rp => rp.Permission.Code).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PermissionDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }
}