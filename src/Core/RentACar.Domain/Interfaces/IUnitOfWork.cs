using RentACar.Domain.Entities;

namespace RentACar.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Özel repository gerektirmeyenler için Generic Repository kullanımı
        IGenericRepository<Car> Cars { get; }
        IGenericRepository<Rental> Rentals { get; }
        
        Task<int> SaveChangesAsync();
    }
}