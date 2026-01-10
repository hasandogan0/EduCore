using System.Linq.Expressions;

namespace EduCore.DataAccess.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity); // Usually update is synchronous in EF Core context tracking but creating async wrapper for consistency
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
