using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Infrastructure.Contexts
{
    public class RentACarDbContext : DbContext
    {
        public RentACarDbContext(DbContextOptions<RentACarDbContext> options) : base(options)
        {
        }

        // DbSet Tanımlamaları
        public DbSet<Car> Cars { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CAR AYARLARI
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.DailyPrice).HasColumnType("decimal(18,2)");
                entity.Property(c => c.Plate).IsRequired().HasMaxLength(20);
                
                // Araç - Marka İlişkisi
                entity.HasOne(c => c.Brand)
                      .WithMany()
                      .HasForeignKey(c => c.BrandId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Araç - Mevcut Şube İlişkisi
                entity.HasOne(c => c.CurrentLocation)
                      .WithMany()
                      .HasForeignKey(c => c.CurrentLocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // RENTAL (KİRALAMA/REZERVASYON) AYARLARI
            modelBuilder.Entity<Rental>(entity =>
            {
                entity.Property(r => r.TotalAmount).HasColumnType("decimal(18,2)");
                
                // Kiralama - Araç İlişkisi
                entity.HasOne(r => r.Car)
                      .WithMany()
                      .HasForeignKey(r => r.CarId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Kiralama - Müşteri İlişkisi
                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Kiralama - Alış Şubesi İlişkisi
                entity.HasOne(r => r.PickUpLocation)
                      .WithMany()
                      .HasForeignKey(r => r.PickUpLocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Kiralama - Dönüş Şubesi İlişkisi
                entity.HasOne(r => r.DropOffLocation)
                      .WithMany()
                      .HasForeignKey(r => r.DropOffLocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ADDITIONAL SERVICE AYARLARI
            modelBuilder.Entity<AdditionalService>(entity =>
            {
                entity.Property(a => a.DailyPrice).HasColumnType("decimal(18,2)");
            });

            // CUSTOMER AYARLARI
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.IdentityNumber).HasMaxLength(11);
                entity.Property(c => c.PhoneNumber).HasMaxLength(20);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}