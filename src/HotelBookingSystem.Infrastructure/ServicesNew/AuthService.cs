using HotelBookingSystem.Application.DTOsNew.AuthNew;
using HotelBookingSystem.Application.ExceptionsNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Infrastructure.ConfigurationsNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HotelBookingSystem.Infrastructure.ServicesNew;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        AppDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null || !user.IsActive)
            throw new BadRequestException("Email yoki parol noto‘g‘ri.");

        var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new BadRequestException("Email yoki parol noto‘g‘ri.");

        var roleNames = await _context.AppUserRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissions = await _context.AppUserRoles
            .Where(x => x.UserId == user.Id)
            .SelectMany(x => x.Role.RolePermissions.Select(rp => rp.Permission.Code))
            .Distinct()
            .ToListAsync(cancellationToken);

        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _tokenService.CreateToken(user, roleNames, permissions, refreshTokenValue);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (existingRefreshToken is null)
            throw new BadRequestException("Refresh token topilmadi.");

        if (existingRefreshToken.IsRevoked)
            throw new BadRequestException("Refresh token bekor qilingan.");

        if (existingRefreshToken.ExpiresAtUtc <= DateTime.UtcNow)
            throw new BadRequestException("Refresh token muddati tugagan.");

        var user = await _context.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == existingRefreshToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
            throw new BadRequestException("User faol emas yoki topilmadi.");

        var roleNames = await _context.AppUserRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissions = await _context.AppUserRoles
            .Where(x => x.UserId == user.Id)
            .SelectMany(x => x.Role.RolePermissions.Select(rp => rp.Permission.Code))
            .Distinct()
            .ToListAsync(cancellationToken);

        existingRefreshToken.IsRevoked = true;
        existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;

        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _tokenService.CreateToken(user, roleNames, permissions, newRefreshTokenValue);
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (existingRefreshToken is null)
            throw new NotFoundException("Refresh token topilmadi.");

        if (!existingRefreshToken.IsRevoked)
        {
            existingRefreshToken.IsRevoked = true;
            existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}