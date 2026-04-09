using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;
using RentACar.Infrastructure.Contexts;

// Veritabanı transaction işlemlerini ve memory yönetimini bu sınıf üstlenecek.
// UnitOfWork, birden fazla repository'yi tek bir transaction içinde yönetmek için kullanılan bir yapıdır. 
// Örneğin, bir araba kiralama işlemi sırasında hem Car tablosunda hem de Rental tablosunda değişiklik
// yapmamız gerekebilir. UnitOfWork sayesinde bu işlemleri tek bir transaction içinde gerçekleştirebiliriz.

namespace RentACar.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RentACarDbContext _context;

        public UnitOfWork(RentACarDbContext context)
        {
            _context = context;
            
            // Generic Repository atamaları
            Cars = new GenericRepository<Car>(_context); // Car tablosu için GenericRepository kullanarak bir repository oluşturuyoruz
            Rentals = new GenericRepository<Rental>(_context);
            Brands = new GenericRepository<Brand>(_context); 
            Locations = new GenericRepository<Location>(_context); 
            Customers = new GenericRepository<Customer>(_context); 
        }

        public IGenericRepository<Car> Cars { get; } 
        public IGenericRepository<Rental> Rentals { get; }
        public IGenericRepository<Brand> Brands { get; } 
        public IGenericRepository<Location> Locations { get; } 
        public IGenericRepository<Customer> Customers { get; } 

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this); // Çöp toplayıcısına (Garbage Collector) işi bittiğini bildirir
        }
    }
}