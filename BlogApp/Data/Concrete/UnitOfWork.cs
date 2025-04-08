using BlogApp.Data.Abstract;

namespace BlogApp.Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogAppDbContext _context;

        private EfCoreBlogPostRepository? _blogPostRepository;
        private EfCoreCategoryRepository? _categoryRepository;
        private EfCoreCommentRepository? _commentRepository; 

        public UnitOfWork(BlogAppDbContext context)
        {
            _context = context;
        }

       
        public IBlogPostRepository BlogPosts =>
            _blogPostRepository ??= new EfCoreBlogPostRepository(_context);

        public ICategoryRepository Categories =>
            _categoryRepository ??= new EfCoreCategoryRepository(_context);

     
        public ICommentRepository Comments =>
             _commentRepository ??= new EfCoreCommentRepository(_context);
       

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
