using BlogApp.DTOs;

namespace BlogApp.Services.Abstract
{
    public interface ICommentService
    {
        Task<IList<CommentDto>> GetCommentsByPostIdAsync(int postId);
        Task<CommentDto?> CreateCommentAsync(CommentCreateDto commentCreateDto, string userId);
        Task<bool> DeleteCommentAsync(int commentId, string userId); // Yetki kontrolü için
        Task<bool> UserOwnsCommentAsync(int commentId, string userId);
    }
}
