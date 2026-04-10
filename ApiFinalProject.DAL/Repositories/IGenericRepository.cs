using System.Linq.Expressions;

namespace ApiFinalProject.DAL.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> GetQueryable();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string id); // For string IDs like IdentityUser
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
