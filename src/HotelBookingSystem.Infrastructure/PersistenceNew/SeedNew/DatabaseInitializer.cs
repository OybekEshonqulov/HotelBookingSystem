using BCrypt.Net;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Domain.EntitiesNew;
using HotelBookingSystem.Domain.EnumsNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.PersistenceNew.SeedNew;

public static class DatabaseInitializer
{
    public static async Task SeedAsync(AppDbContext context, bool applyMigrations = true)
    {
        if (applyMigrations)
            await context.Database.MigrateAsync();

        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new() { Code = PermissionCodes.TenantsView, Name = "View tenants" },
                new() { Code = PermissionCodes.TenantsCreate, Name = "Create tenants" },
                new() { Code = PermissionCodes.TenantsEdit, Name = "Edit tenants" },

                new() { Code = PermissionCodes.UsersView, Name = "View users" },
                new() { Code = PermissionCodes.UsersCreate, Name = "Create users" },
                new() { Code = PermissionCodes.UsersEdit, Name = "Edit users" },

                new() { Code = PermissionCodes.RolesView, Name = "View roles" },
                new() { Code = PermissionCodes.RolesCreate, Name = "Create roles" },
                new() { Code = PermissionCodes.RolesEdit, Name = "Edit roles" },

                new() { Code = PermissionCodes.PropertiesView, Name = "View properties" },
                new() { Code = PermissionCodes.PropertiesCreate, Name = "Create properties" },
                new() { Code = PermissionCodes.PropertiesEdit, Name = "Edit properties" },

                new() { Code = PermissionCodes.ReservationsView, Name = "View reservations" },
                new() { Code = PermissionCodes.ReservationsCreate, Name = "Create reservations" },
                new() { Code = PermissionCodes.ReservationsEdit, Name = "Edit reservations" },
                new() { Code = PermissionCodes.ReservationsCancel, Name = "Cancel reservations" },

                new() { Code = PermissionCodes.PaymentsView, Name = "View payments" },
                new() { Code = PermissionCodes.PaymentsCreate, Name = "Create payments" },

                new() { Code = PermissionCodes.ReportsView, Name = "View reports" }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        var superTenant = await context.Tenants.FirstOrDefaultAsync(x => x.Name == "System");
        if (superTenant is null)
        {
            superTenant = new Tenant
            {
                Name = "System",
                Type = TenantType.Hotel,
                Status = PropertyStatus.Active,
                CurrencyCode = "UZS",
                TimeZone = "Asia/Tashkent",
                Email = "system@local"
            };

            await context.Tenants.AddAsync(superTenant);
            await context.SaveChangesAsync();
        }

        var superAdminRole = await context.AppRoles.FirstOrDefaultAsync(x =>
            x.TenantId == superTenant.Id && x.Name == "SuperAdmin");

        if (superAdminRole is null)
        {
            superAdminRole = new AppRole
            {
                TenantId = superTenant.Id,
                Name = "SuperAdmin",
                Description = "Full system access"
            };

            await context.AppRoles.AddAsync(superAdminRole);
            await context.SaveChangesAsync();
        }

        var permissionsInDb = await context.Permissions.ToListAsync();
        foreach (var permission in permissionsInDb)
        {
            var exists = await context.RolePermissions.AnyAsync(x =>
                x.RoleId == superAdminRole.Id && x.PermissionId == permission.Id);

            if (!exists)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync();

        var adminEmail = "admin@hotel.local";
        var superAdminUser = await context.AppUsers.FirstOrDefaultAsync(x => x.Email == adminEmail);

        if (superAdminUser is null)
        {
            superAdminUser = new AppUser
            {
                TenantId = superTenant.Id,
                FirstName = "System",
                LastName = "Admin",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsActive = true
            };

            await context.AppUsers.AddAsync(superAdminUser);
            await context.SaveChangesAsync();
        }

        var userRoleExists = await context.AppUserRoles.AnyAsync(x =>
            x.UserId == superAdminUser.Id && x.RoleId == superAdminRole.Id);

        if (!userRoleExists)
        {
            await context.AppUserRoles.AddAsync(new AppUserRole
            {
                UserId = superAdminUser.Id,
                RoleId = superAdminRole.Id
            });

            await context.SaveChangesAsync();
        }
    }
}