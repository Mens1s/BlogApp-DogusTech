using System.Diagnostics;
using BlogApp.Services.Abstract;
using BlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogPostService _blogPostService; // Blog post servisini ekledik

        public HomeController(ILogger<HomeController> logger, IBlogPostService blogPostService)
        {
            _logger = logger;
            _blogPostService = blogPostService; 
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var blogPosts = await _blogPostService.GetAllBlogPostsAsync();

                return View(blogPosts); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching blog posts for the homepage.");
                return RedirectToAction("Error"); 
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
