using RentACar.Domain.Entities;

namespace RentACar.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Özel repository gerektirmeyenler için Generic Repository kullanımı
        IGenericRepository<Car> Cars { get; }
        IGenericRepository<Rental> Rentals { get; }
        IGenericRepository<Brand> Brands { get; } 
        IGenericRepository<Location> Locations { get; } 
        IGenericRepository<Customer> Customers { get; } 
        Task<int> SaveChangesAsync();
    }
}