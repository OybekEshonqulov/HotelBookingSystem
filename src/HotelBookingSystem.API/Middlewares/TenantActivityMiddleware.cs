using HotelBookingSystem.Infrastructure.PersistenceNew;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelBookingSystem.API.Middlewares;

public class TenantActivityMiddleware
{
    private readonly RequestDelegate _next;

    public TenantActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var user = context.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var isSuperAdmin = user.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .Any(x => x.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase));

            if (!isSuperAdmin)
            {
                var tenantIdValue = user.FindFirst("tenant_id")?.Value;

                if (!string.IsNullOrWhiteSpace(tenantIdValue) && Guid.TryParse(tenantIdValue, out var tenantId))
                {
                    var tenant = await dbContext.Tenants
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == tenantId);

                    if (tenant is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Tenant topilmadi."
                        });
                        return;
                    }

                    if (tenant.Status != HotelBookingSystem.Domain.EnumsNew.PropertyStatus.Active)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Ushbu mehmonxona kabineti nofaol qilingan."
                        });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}