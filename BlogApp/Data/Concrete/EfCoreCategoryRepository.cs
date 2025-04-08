using BlogApp.Data.Abstract;
using BlogApp.Entities;

namespace BlogApp.Data.Concrete
{
    public class EfCoreCategoryRepository: EfCoreRepository<Category>, ICategoryRepository
    {
        public EfCoreCategoryRepository(BlogAppDbContext context) : base(context)
        {
        }
    }
}
