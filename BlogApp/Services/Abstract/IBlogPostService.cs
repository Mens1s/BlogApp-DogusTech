using BlogApp.DTOs;

namespace BlogApp.Services.Abstract
{
    public interface IBlogPostService
    {
        Task<BlogPostDetailDto?> GetBlogPostByIdAsync(int id);
        Task<IList<BlogPostDto>> GetAllBlogPostsAsync();
        Task<IList<BlogPostDto>> GetBlogPostsByCategoryAsync(int categoryId);
        Task<BlogPostDto?> CreateBlogPostAsync(BlogPostCreateDto blogPostCreateDto, string userId); 
        Task<bool> UpdateBlogPostAsync(BlogPostUpdateDto blogPostUpdateDto, string userId); 
        Task<bool> DeleteBlogPostAsync(int id, string userId);
        Task<bool> UserOwnsPostAsync(int postId, string userId);
    }
}
