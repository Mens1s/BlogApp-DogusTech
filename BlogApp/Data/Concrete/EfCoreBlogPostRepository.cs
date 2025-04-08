using BlogApp.Data.Abstract;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete
{
    public class EfCoreBlogPostRepository: EfCoreRepository<BlogPost>, IBlogPostRepository
    {
        public EfCoreBlogPostRepository(BlogAppDbContext context) : base(context)
        {
            
        }

        private BlogAppDbContext? BlogAppContext => _context as BlogAppDbContext;


        public async Task<IList<BlogPost>> GetAllByCategoryWithDetailsAsync(int categoryId)
        {
            // Null check önemli!
            if (BlogAppContext == null) return new List<BlogPost>();

            return await BlogAppContext.BlogPosts
                .Include(p => p.User)       
                .Include(p => p.Category)  
                .Where(p => p.CategoryId == categoryId) 
                .OrderByDescending(p => p.Id) // Yeniden eskiye sırala yapilabilir datetime felan
                .ToListAsync();
        }

        public async Task<IList<BlogPost>> GetAllWithDetailsAsync()
        {
            if (BlogAppContext == null) return new List<BlogPost>();

            return await BlogAppContext.BlogPosts
                .Include(p => p.User)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdWithDetailsAsync(int id)
        {
            if (BlogAppContext == null) return null;

            return await BlogAppContext.BlogPosts
               .Include(p => p.User)
               .Include(p => p.Category)
               .Include(p => p.Comments) 
               .ThenInclude(c => c.User) 
               .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}
