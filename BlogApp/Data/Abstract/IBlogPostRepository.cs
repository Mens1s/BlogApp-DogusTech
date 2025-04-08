using BlogApp.Entities;

namespace BlogApp.Data.Abstract
{
    // TODO : Pageable
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IList<BlogPost>> GetAllByCategoryWithDetailsAsync(int categoryId);

        Task<IList<BlogPost>> GetAllWithDetailsAsync();

        Task<BlogPost?> GetByIdWithDetailsAsync(int id);
    }
}
