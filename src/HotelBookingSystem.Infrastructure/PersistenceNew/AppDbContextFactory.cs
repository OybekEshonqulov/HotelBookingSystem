using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelBookingSystem.Infrastructure.PersistenceNew;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql("Host=45.138.159.86;Port=5432;Database=oybekdb;Username=oybek;Password=oybek!2309;");

        return new AppDbContext(optionsBuilder.Options);
    }
}