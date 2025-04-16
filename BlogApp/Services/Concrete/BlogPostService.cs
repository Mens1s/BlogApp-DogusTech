using AutoMapper;
using BlogApp.Data.Abstract;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Services.Abstract;

namespace BlogApp.Services.Concrete
{

    public class BlogPostService : IBlogPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // AutoMapper instance'ı
        private readonly ILogger<BlogPostService> _logger; // Logging için

        // Constructor Injection ile bağımlılıkları alıyoruz
        public BlogPostService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BlogPostService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BlogPostDto?> CreateBlogPostAsync(BlogPostCreateDto blogPostCreateDto, string userId)
        {
            try
            {
                var blogPost = _mapper.Map<BlogPost>(blogPostCreateDto);
                blogPost.UserId = userId;
                                              
                await _unitOfWork.BlogPosts.AddAsync(blogPost);
                var result = await _unitOfWork.SaveChangesAsync(); 

                if (result > 0)
                {
                    return _mapper.Map<BlogPostDto>(blogPost);
                }
                _logger.LogWarning("Blog post could not be created. SaveChanges returned 0.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating blog post for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> DeleteBlogPostAsync(int id, string userId)
        {
            try
            {
                var blogPost = await _unitOfWork.BlogPosts.GetAsync(p => p.Id == id && p.UserId == userId);

                if (blogPost == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent or unauthorized blog post. PostId: {PostId}, UserId: {UserId}", id, userId);
                    return false; 
                }

                await _unitOfWork.BlogPosts.DeleteAsync(blogPost);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting blog post {PostId} by user {UserId}", id, userId);
                return false;
            }
        }

        public async Task<IList<BlogPostDto>> GetAllBlogPostsAsync()
        {
            try
            {
                var blogPosts = await _unitOfWork.BlogPosts.GetAllWithDetailsAsync();
                return _mapper.Map<IList<BlogPostDto>>(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all blog posts.");
                return new List<BlogPostDto>(); 
            }
        }

        public async Task<BlogPostDetailDto?> GetBlogPostByIdAsync(int id)
        {
            try
            {
                // Belirli ID'ye sahip postu ilişkili verilerle çek
                var blogPost = await _unitOfWork.BlogPosts.GetByIdWithDetailsAsync(id);
                if (blogPost == null)
                {
                    return null; // Bulunamadı
                }
                // Entity'yi Detail DTO'ya map'le
                return _mapper.Map<BlogPostDetailDto>(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting blog post with Id {PostId}", id);
                return null;
            }
        }

        public async Task<IList<BlogPostDto>> GetBlogPostsByCategoryAsync(int categoryId)
        {
            try
            {
                var blogPosts = await _unitOfWork.BlogPosts.GetAllByCategoryWithDetailsAsync(categoryId);
                return _mapper.Map<IList<BlogPostDto>>(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting blog posts for category {CategoryId}", categoryId);
                return new List<BlogPostDto>();
            }
        }

        public async Task<bool> UpdateBlogPostAsync(BlogPostUpdateDto blogPostUpdateDto, string userId)
        {
            try
            {
                var existingPost = await _unitOfWork.BlogPosts.GetAsync(p => p.Id == blogPostUpdateDto.Id);

                if (existingPost == null)
                {
                    _logger.LogWarning("Attempted to update non-existent blog post. PostId: {PostId}", blogPostUpdateDto.Id);
                    return false; // Post bulunamadı
                }

                if (existingPost.UserId != userId)
                {
                    _logger.LogWarning("Unauthorized attempt to update blog post. PostId: {PostId}, UserId: {UserId}", blogPostUpdateDto.Id, userId);
                    return false; 
                }


                _mapper.Map(blogPostUpdateDto, existingPost);

                await _unitOfWork.BlogPosts.UpdateAsync(existingPost);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating blog post {PostId} by user {UserId}", blogPostUpdateDto.Id, userId);
                return false;
            }
        }

        public async Task<bool> UserOwnsPostAsync(int postId, string userId)
        {
            return await _unitOfWork.BlogPosts.AnyAsync(p => p.Id == postId && p.UserId == userId);
        }
    }
    
}
