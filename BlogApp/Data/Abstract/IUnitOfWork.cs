namespace BlogApp.Data.Abstract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IBlogPostRepository BlogPosts { get; }
        ICategoryRepository Categories { get; }
        ICommentRepository Comments { get; } 

        Task<int> SaveChangesAsync();
    }
}
