using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;
using RentACar.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Generic Repository (Genel Amaçlı Veri Erişim Katmanı) mantığıyla yazılmış bir sınıf. Yani 
// veritabanındaki farklı tablolarda tekrar tekrar aynı kodları yazmamak için tek bir yapı oluştururuz.
// GenericRepository, tüm entity'ler için ortak CRUD işlemlerini sağlayan bir repository sınıfıdır. 
// Örneğin, Car tablosu için CarRepository yazmak yerine, GenericRepository<Car> kullanarak aynı 
// işlemleri gerçekleştirebiliriz. Bu sayede kod tekrarını önler ve bakım kolaylığı sağlar.

namespace RentACar.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly RentACarDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(RentACarDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

      
        public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => !x.IsDeleted);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => !x.IsDeleted);
            
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Önce filtrelenmiş verinin TOPLAM sayısını alıyoruz (Sayfalama hesabı için gerekli)
            var totalCount = await query.CountAsync();

            // Sonra Skip ve Take ile SADECE o sayfaya ait verileri SQL'den çekiyoruz
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate).Where(x => !x.IsDeleted);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity)
        {
            // Soft delete mantığı
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(x => !x.IsDeleted).CountAsync();
        }

        public async Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.Where(predicate).Where(x => !x.IsDeleted).SumAsync(selector);
        }
    }
}