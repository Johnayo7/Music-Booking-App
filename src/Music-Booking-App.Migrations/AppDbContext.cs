
using Music_Booking_App.Models.Entiites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Music_Booking_App.Migrations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Test> Tests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OTP> OTPs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<byte[], long>(
            v => BitConverter.ToInt64(v ?? new byte[8], 0),  // Provide a default empty byte array if v is null
            v => BitConverter.GetBytes(v));

            var comparer = new ValueComparer<byte[]>(
                (b1, b2) => BitConverter.ToInt64(b1 ?? new byte[8], 0) == BitConverter.ToInt64(b2 ?? new byte[8], 0), // Handle null for b1 and b2
                b => b == null ? 0 : BitConverter.ToInt64(b, 0).GetHashCode(),
                b => b == null ? new byte[8] : BitConverter.GetBytes(BitConverter.ToInt64(b, 0)));  // Ensure a non-null byte array is returned


            // Apply the TimeStamp conversion to all relevant entities
            void ApplyTimeStampConversion<T>(EntityTypeBuilder<T> entity) where T : class
            {
                entity.Property("TimeStamp")
                      .HasColumnName("xmin")
                      .HasColumnType("xid")
                      .HasConversion(converter)
                      .Metadata.SetValueComparer(comparer);
            }
            ApplyTimeStampConversion(modelBuilder.Entity<Test>());
            ApplyTimeStampConversion(modelBuilder.Entity<User>());
            ApplyTimeStampConversion(modelBuilder.Entity<OTP>());



            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal))
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name).HasPrecision(18, 2);
                    }
                }
            }

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.HasKey(u => u.Id);
                b.Property(u => u.OrganizationId).IsRequired(false);

                // Configure lockout properties
                b.Property(u => u.AccessFailedCount).HasDefaultValue(0);
                b.Property(u => u.LockoutEnabled).HasDefaultValue(true);
                b.Property(u => u.LockoutEnd).HasDefaultValue(null);

                // Ensure PasswordHash and PasswordSalt are treated as binary data (byte[])
                b.Property(u => u.PasswordHash)
                    .HasColumnType("bytea")
                    .IsRequired();

                b.Property(u => u.PasswordSalt)
                    .HasColumnType("bytea")
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
