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
        var tenantId = GetTenantId();

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Role nomi kiritilishi shart.");

        var exists = await _context.AppRoles.AnyAsync(x =>
            x.TenantId == tenantId && x.Name == request.Name.Trim(), cancellationToken);

        if (exists)
            throw new ConflictException("Bu nomdagi role allaqachon mavjud.");

        var permissions = await _context.Permissions
            .Where(x => request.PermissionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var role = new AppRole
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
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
        return await BuildRoleDtoAsync(role.Id, cancellationToken);
    }

    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

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
                Permissions = x.RolePermissions
                    .OrderBy(rp => rp.Permission.Code)
                    .Select(rp => rp.Permission.Code)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var exists = await _context.AppRoles
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

        if (!exists)
            return null;

        return await BuildRoleDtoAsync(id, cancellationToken);
    }

    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var role = await _context.AppRoles
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

        if (role is null)
            throw new NotFoundException("Role topilmadi.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Role nomi kiritilishi shart.");

        var duplicate = await _context.AppRoles.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.Id != id &&
            x.Name == request.Name.Trim(),
            cancellationToken);

        if (duplicate)
            throw new ConflictException("Bu nomdagi role allaqachon mavjud.");

        var permissions = await _context.Permissions
            .Where(x => request.PermissionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        role.Name = request.Name.Trim();
        role.Description = request.Description?.Trim();

        var oldPermissions = await _context.RolePermissions
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

        _context.RolePermissions.RemoveRange(oldPermissions);

        foreach (var permission in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildRoleDtoAsync(role.Id, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var role = await _context.AppRoles
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

        if (role is null)
            throw new NotFoundException("Role topilmadi.");

        var assignedToUsers = await _context.AppUserRoles
            .AnyAsync(x => x.RoleId == role.Id, cancellationToken);

        if (assignedToUsers)
            throw new ConflictException("Bu role userlarga biriktirilgan. Avval userlardan olib tashlang.");

        var rolePermissions = await _context.RolePermissions
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

        _context.RolePermissions.RemoveRange(rolePermissions);
        _context.AppRoles.Remove(role);

        await _context.SaveChangesAsync(cancellationToken);
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

    private Guid GetTenantId()
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        return _currentUserService.TenantId.Value;
    }

    private async Task<RoleDto> BuildRoleDtoAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _context.AppRoles
            .AsNoTracking()
            .Where(x => x.Id == roleId)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                Name = x.Name,
                Description = x.Description,
                Permissions = x.RolePermissions
                    .OrderBy(rp => rp.Permission.Code)
                    .Select(rp => rp.Permission.Code)
                    .ToList()
            })
            .FirstAsync(cancellationToken);
    }
}