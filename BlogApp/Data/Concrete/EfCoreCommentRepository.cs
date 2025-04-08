using BlogApp.Data.Abstract;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete
{
    public class EfCoreCommentRepository : EfCoreRepository<Comment>, ICommentRepository
    {
        public EfCoreCommentRepository(BlogAppDbContext context) : base(context)
        {
        }

        private BlogAppDbContext? BlogAppContext => _context as BlogAppDbContext;

        public async Task<IList<Comment>> GetAllByPostIdWithUserAsync(int postId)
        {
            if (BlogAppContext == null) return new List<Comment>();

            return await BlogAppContext.Comments
               .Include(c => c.User)
               .Where(c => c.BlogPostId == postId)
               .OrderByDescending(c => c.CommentDate) 
               .ToListAsync();
        }

        public async Task<IList<Comment>> GetAllByUserIdAsync(string userId)
        {
            if (BlogAppContext == null) return new List<Comment>();

            return await BlogAppContext.Comments
               .Where(c => c.UserId == userId) 
               .OrderByDescending(c => c.CommentDate)
               .ToListAsync();
        }
    }
}
