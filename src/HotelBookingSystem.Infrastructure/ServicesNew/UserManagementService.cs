using HotelBookingSystem.Application.DTOsNew.CommonNew;
using HotelBookingSystem.Application.DTOsNew.UserManagementNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordService _passwordService;

    public UserManagementService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        IPasswordService passwordService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordService = passwordService;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenantId = ResolveTenantId(request.TenantId);

        var exists = await _context.AppUsers.AnyAsync(x =>
            x.TenantId == tenantId && x.Email == request.Email.Trim(), cancellationToken);

        if (exists)
            throw new ConflictException("Bu email bilan user allaqachon mavjud.");

        var roles = await _context.AppRoles
            .Where(x => x.TenantId == tenantId && request.RoleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = _passwordService.HashPassword(request.Password),
            IsActive = true
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var role in roles)
        {
            _context.AppUserRoles.Add(new AppUserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await BuildUserDtoAsync(user.Id, cancellationToken);
    }

    public async Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = _context.AppUsers.AsNoTracking().AsQueryable();

        if (_currentUserService.IsSuperAdmin)
        {
            if (request.TenantId.HasValue)
                query = query.Where(x => x.TenantId == request.TenantId.Value);
        }
        else
        {
            var tenantId = GetCurrentTenantId();
            query = query.Where(x => x.TenantId == tenantId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                (x.FirstName + " " + x.LastName).ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<UserDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items
        };
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null || !CanAccessTenant(user.TenantId))
            return null;

        return await BuildUserDtoAsync(id, cancellationToken);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null || !CanAccessTenant(user.TenantId))
            throw new NotFoundException("User topilmadi.");

        var exists = await _context.AppUsers.AnyAsync(x =>
            x.TenantId == user.TenantId &&
            x.Id != id &&
            x.Email == request.Email.Trim(),
            cancellationToken);

        if (exists)
            throw new ConflictException("Bu email bilan boshqa user mavjud.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildUserDtoAsync(user.Id, cancellationToken);
    }

    public async Task<UserDto> UpdateStatusAsync(Guid id, UpdateUserStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null || !CanAccessTenant(user.TenantId))
            throw new NotFoundException("User topilmadi.");

        user.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildUserDtoAsync(user.Id, cancellationToken);
    }

    public async Task<UserDto> AssignRolesAsync(AssignRolesRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null || !CanAccessTenant(user.TenantId))
            throw new NotFoundException("User topilmadi.");

        var roles = await _context.AppRoles
            .Where(x => x.TenantId == user.TenantId && request.RoleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var oldUserRoles = await _context.AppUserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        _context.AppUserRoles.RemoveRange(oldUserRoles);

        foreach (var role in roles)
        {
            _context.AppUserRoles.Add(new AppUserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildUserDtoAsync(user.Id, cancellationToken);
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
        return _currentUserService.IsSuperAdmin || (_currentUserService.TenantId.HasValue && _currentUserService.TenantId.Value == tenantId);
    }

    private async Task<UserDto> BuildUserDtoAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.AppUsers
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new UserDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstAsync(cancellationToken);
    }
}