using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using BlogApp.Data.Abstract;

namespace BlogApp.Data.Concrete
{
    public class EfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public EfCoreRepository(DbContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate); 
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty); 
                }
            }

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;
            query = query.Where(predicate); 

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty); 
                }
            }

            // SingleOrDefaultAsync: Koşula uyan tek bir eleman olmalı, birden fazla varsa hata verir.
            // FirstOrDefaultAsync: Koşula uyan ilk elemanı döner, birden fazla varsa hata vermez.
            // Genellikle GetAsync için SingleOrDefaultAsync daha mantıklıdır.
            return await query.SingleOrDefaultAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            // FindAsync, primary key'e göre arama yapar ve genellikle en hızlı yöntemdir.
            // Cache'lenmiş veriyi de kontrol eder.
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<TEntity?> GetByIdAsync(string id) 
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual Task UpdateAsync(TEntity entity) // Async değil çünkü sadece state'i değiştiriyor
        {
            // Entity'nin state'ini Modified olarak işaretle. EF Core tüm alanları günceller.
            _context.Entry(entity).State = EntityState.Modified;
            // Alternatif (daha kontrollü): Attach edip sadece değişen property'leri işaretlemek.
            // _dbSet.Attach(entity);
            // _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask; // SaveChangesAsync UoW'da çağrılacak
        }
    }
}
