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
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var exists = await _context.AppUsers.AnyAsync(x =>
            x.TenantId == tenantId && x.Email == request.Email, cancellationToken);

        if (exists)
            throw new ConflictException("Bu email bilan user allaqachon mavjud.");

        var roles = await _context.AppRoles
            .Where(x => x.TenantId == tenantId && request.RoleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var user = new AppUser
        {
            TenantId = tenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
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

        return await _context.AppUsers
            .AsNoTracking()
            .Where(x => x.Id == user.Id)
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

    public async Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new BadRequestException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var query = _context.AppUsers
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .AsQueryable();

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

    public async Task<UserDto> AssignRolesAsync(AssignRolesRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.TenantId.HasValue)
            throw new NotFoundException("Tenant aniqlanmadi.");

        var tenantId = _currentUserService.TenantId.Value;

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == request.UserId && x.TenantId == tenantId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User topilmadi.");

        var roles = await _context.AppRoles
            .Where(x => x.TenantId == tenantId && request.RoleIds.Contains(x.Id))
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

        return await _context.AppUsers
            .AsNoTracking()
            .Where(x => x.Id == user.Id)
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