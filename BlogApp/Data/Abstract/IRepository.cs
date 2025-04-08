using System.Linq.Expressions;

namespace BlogApp.Data.Abstract
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id); // identity user icin..

        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        // ilisikili nesneler => includeProp.. predicate ise T nesnesine
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
