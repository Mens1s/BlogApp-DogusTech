using BlogApp.Entities;

namespace BlogApp.Data.Abstract
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IList<Comment>> GetAllByPostIdWithUserAsync(int postId);
        Task<IList<Comment>> GetAllByUserIdAsync(string userId);
    }
}
