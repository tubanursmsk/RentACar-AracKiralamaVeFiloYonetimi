using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Infrastructure.Contexts
{
    public class RentACarDbContext : DbContext
    {
        public RentACarDbContext(DbContextOptions<RentACarDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BaseEntity'den gelen ortak konfigürasyonlar veya 
            // Car/Rental için özel kısıtlamalar (Fluent API) buraya yazılabilir.
            
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.DailyPrice).HasColumnType("decimal(18,2)");
                entity.Property(c => c.Plate).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.Property(r => r.TotalAmount).HasColumnType("decimal(18,2)");
                
                // Car ile Rental arasındaki ilişki
                entity.HasOne(r => r.Car)
                      .WithMany()
                      .HasForeignKey(r => r.CarId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}