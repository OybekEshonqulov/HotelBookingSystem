using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.PersistenceNew;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppRole> AppRoles => Set<AppRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<PropertyReview> PropertyReviews => Set<PropertyReview>(); public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationItem> ReservationItems => Set<ReservationItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("hotel_booking");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}