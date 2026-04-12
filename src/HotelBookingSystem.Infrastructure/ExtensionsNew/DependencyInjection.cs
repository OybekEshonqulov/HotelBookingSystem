using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Infrastructure.PersistenceNew;
using HotelBookingSystem.Infrastructure.ServicesNew;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem.Infrastructure.ExtensionsNew;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
     options.UseNpgsql(connectionString, x =>
         x.MigrationsHistoryTable("__EFMigrationsHistory", "hotel_booking")));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBedService, BedService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICheckInOutService, CheckInOutService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IHousekeepingService, HousekeepingService>();
        services.AddScoped<IRoleManagementService, RoleManagementService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IPublicCatalogService, PublicCatalogService>();
        services.AddScoped<IPublicBookingService, PublicBookingService>();
        return services;
    }
}