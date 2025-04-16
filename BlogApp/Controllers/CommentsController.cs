using AutoMapper;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Authorize] // Yorum işlemleri için giriş zorunlu
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CommentsController> _logger;
        private readonly IMapper _mapper; // Belki lazım olabilir

        public CommentsController(ICommentService commentService, UserManager<User> userManager, ILogger<CommentsController> logger, IMapper mapper)
        {
            _commentService = commentService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Model geçerli değilse ne yapmalı? Genellikle AJAX ile gönderilir.
                // Şimdilik hata mesajı ile geri yönlendirelim.
                TempData["ErrorMessage"] = "Yorum içeriği boş olamaz veya çok uzun.";
                // Yorumun ait olduğu post ID'sini bilmemiz lazım! DTO'da var.
                return RedirectToAction("Details", "BlogPosts", new { id = dto.BlogPostId });
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge(); // Giriş yapılmamış

            try
            {
                var createdComment = await _commentService.CreateCommentAsync(dto, userId);
                if (createdComment != null)
                {
                    _logger.LogInformation("Comment created successfully for post {PostId} by user {UserId}", dto.BlogPostId, userId);
                    TempData["SuccessMessage"] = "Yorumunuz başarıyla eklendi.";
                }
                else
                {
                    _logger.LogError("Failed to create comment for post {PostId} by user {UserId}", dto.BlogPostId, userId);
                    TempData["ErrorMessage"] = "Yorum eklenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception creating comment for post {PostId} by user {UserId}", dto.BlogPostId, userId);
                TempData["ErrorMessage"] = "Yorum eklenirken beklenmedik bir hata oluştu.";
            }


            // Her durumda postun detay sayfasına geri dön
            return RedirectToAction("Details", "BlogPosts", new { id = dto.BlogPostId });
        }


        // POST: Comments/Delete/123
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId, int blogPostId) // Yönlendirme için blogPostId de alalım
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            try
            {
                // Yetki kontrolü serviste veya burada yapılabilir:
                // bool owns = await _commentService.UserOwnsCommentAsync(commentId, userId);
                // if (!owns && !User.IsInRole("Admin")) return Forbid();

                bool success = await _commentService.DeleteCommentAsync(commentId, userId); // Servis yetkiyi kontrol etmeli

                if (success)
                {
                    _logger.LogInformation("Comment {CommentId} deleted successfully by user {UserId}", commentId, userId);
                    TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
                }
                else
                {
                    _logger.LogError("Failed to delete comment {CommentId} by user {UserId}", commentId, userId);
                    TempData["ErrorMessage"] = "Yorum silinirken bir hata oluştu veya yetkiniz yok.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception deleting comment {CommentId} by user {UserId}", commentId, userId);
                TempData["ErrorMessage"] = "Yorum silinirken beklenmedik bir hata oluştu.";
            }


            return RedirectToAction("Details", "BlogPosts", new { id = blogPostId });
        }
    }
}
