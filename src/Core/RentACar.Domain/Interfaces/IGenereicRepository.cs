using System.Linq.Expressions;
using RentACar.Domain.Entities;

namespace RentACar.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity 
    {
        Task<T?> GetByIdAsync(int id); // int olarak güncelledik
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector);
        Task AddAsync(T entity); 
        void Update(T entity);
        void Delete(T entity);
    }
}