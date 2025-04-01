

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Music_Booking_App.Migrations
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile(Directory.GetCurrentDirectory() + "/../Music-Booking-App.API/appsettings.json")
           .Build();
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DbConnectionString")!;
            builder.UseNpgsql(connectionString);
            return new AppDbContext(builder.Options);
        }

    }
}
