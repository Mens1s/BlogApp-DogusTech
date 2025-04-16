using AutoMapper;
using BlogApp.Data.Abstract;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Services.Abstract;

namespace BlogApp.Services.Concrete
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CommentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CommentDto?> CreateCommentAsync(CommentCreateDto commentCreateDto, string userId)
        {
            try
            {
                var comment = _mapper.Map<Comment>(commentCreateDto);
                comment.UserId = userId; // Yorumu yapan kullanıcıyı ata

                bool postExists = await _unitOfWork.BlogPosts.AnyAsync(p => p.Id == comment.BlogPostId);
                if (!postExists)
                {
                    _logger.LogWarning("Attempted to add comment to non-existent blog post. PostId: {BlogPostId}", comment.BlogPostId);
                    return null; 
                }

                await _unitOfWork.Comments.AddAsync(comment);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0)
                {
  
                    var commentDto = _mapper.Map<CommentDto>(comment);
                    return commentDto;
                }

                _logger.LogWarning("Comment could not be created for post {BlogPostId}. SaveChanges returned 0.", comment.BlogPostId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating comment for post {BlogPostId} by user {UserId}", commentCreateDto.BlogPostId, userId);
                return null;
            }
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            try
            {
                // Yorumu bul
                var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);

                if (comment == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent comment. CommentId: {CommentId}", commentId);
                    return false; 
                }

                if (comment.UserId != userId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete comment. CommentId: {CommentId}, UserId: {UserId}", commentId, userId);
                    return false; 
                }

                await _unitOfWork.Comments.DeleteAsync(comment);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting comment {CommentId} by user {UserId}", commentId, userId);
                return false;
            }
        }

        public async Task<IList<CommentDto>> GetCommentsByPostIdAsync(int postId)
        {
            try
            {

                var comments = await _unitOfWork.Comments.GetAllByPostIdWithUserAsync(postId);

                return _mapper.Map<IList<CommentDto>>(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting comments for post {PostId}", postId);
                return new List<CommentDto>(); // Hata durumunda boş liste dön
            }
        }

        public async Task<bool> UserOwnsCommentAsync(int commentId, string userId)
        {
            return await _unitOfWork.Comments.AnyAsync(c => c.Id == commentId && c.UserId == userId);
        }
    }
}
