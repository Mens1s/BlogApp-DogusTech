using BlogApp.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BlogApp.Entities;

namespace BlogApp.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostsController> _logger;
        private readonly ICommentService _commentService; // Yorumlar için eklenecek
        private readonly UserManager<User> _userManager; // Yorum eklerken kullanıcı ID'si için

        public BlogPostsController(
            IBlogPostService blogPostService,
            ILogger<BlogPostsController> logger
            , ICommentService commentService 
            , UserManager<User> userManager
            )
        {
            _blogPostService = blogPostService;
            _logger = logger;
            _commentService = commentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id.Value <= 0)
            {
                _logger.LogWarning("Details action called with invalid ID: {Id}", id);
                return NotFound(); // Geçersiz ID veya ID yoksa NotFound döndür
            }

            try
            {
                var blogPostDto = await _blogPostService.GetBlogPostByIdAsync(id.Value);

                if (blogPostDto == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found.", id.Value);
                    return NotFound(); // Post bulunamadıysa NotFound döndür
                }

                var comments = await _commentService.GetCommentsByPostIdAsync(id.Value);
                ViewData["Comments"] = comments; // Yorumları ViewData veya ViewBag ile View'a gönderelim
                

                return View(blogPostDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching details for blog post {Id}", id.Value);
                return StatusCode(500, "İç Sunucu Hatası"); // Veya daha spesifik bir hata
            }
        }


    }
}
