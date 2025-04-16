using BlogApp.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BlogApp.Entities;
using AutoMapper;
using BlogApp.DTOs;
using BlogApp.ViewModels.BlogPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers
{
    [Authorize] // Bu controller'daki tüm action'lar (şimdilik Create) giriş gerektirir
    public class BlogPostsController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ICategoryService _categoryService; 
        private readonly UserManager<User> _userManager; 
        private readonly IMapper _mapper; 
        private readonly ILogger<BlogPostsController> _logger;
        private readonly ICommentService _commentService; 

        public BlogPostsController(
            IBlogPostService blogPostService,
            ICategoryService categoryService, // Inject edildi
            UserManager<User> userManager,     // Inject edildi
            IMapper mapper,                   // Inject edildi
            ILogger<BlogPostsController> logger,
            ICommentService commentService)
        {
            _blogPostService = blogPostService;
            _categoryService = categoryService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _commentService = commentService;
        }

        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BlogPostCreateViewModel
            {
                Categories = await GetCategoriesSelectListAsync() // Kategorileri alıp SelectList yap
            };
            return View(viewModel);
        }

        // POST: BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostCreateViewModel viewModel)
        {
            // Model geçerli değilse veya kategori seçilmemişse formu tekrar göster
            if (!ModelState.IsValid || viewModel.CategoryId <= 0)
            {
                if (viewModel.CategoryId <= 0)
                {
                    ModelState.AddModelError(nameof(viewModel.CategoryId), "Lütfen bir kategori seçin.");
                }
                _logger.LogWarning("Create POST called with invalid model state.");
                viewModel.Categories = await GetCategoriesSelectListAsync(viewModel.CategoryId); // Seçili kategoriyi koru
                return View(viewModel);
            }

            try
            {
                var userId = _userManager.GetUserId(User); // Giriş yapmış kullanıcının ID'sini al
                if (string.IsNullOrEmpty(userId))
                {
                    // Bu durum normalde [Authorize] nedeniyle olmaz ama kontrol iyi.
                    return Challenge(); // Giriş yapmaya zorla
                }

                // ViewModel'ı DTO'ya dönüştür (AutoMapper ile veya manuel)
                var createDto = _mapper.Map<BlogPostCreateDto>(viewModel);

                var createdPost = await _blogPostService.CreateBlogPostAsync(createDto, userId);

                if (createdPost != null)
                {
                    _logger.LogInformation("Blog post {PostId} created successfully by user {UserId}", createdPost.Id, userId);
                    // TempData ile başarı mesajı gösterilebilir
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla oluşturuldu!";
                    return RedirectToAction("Details", new { id = createdPost.Id }); // Oluşturulan postun detayına git
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Blog yazısı oluşturulurken bir hata oluştu.");
                    _logger.LogError("Blog post creation failed for user {UserId}.", userId);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Beklenmedik bir hata oluştu.");
                _logger.LogError(ex, "Exception occurred during blog post creation for user {UserId}.", _userManager.GetUserId(User));
            }

            // Hata durumunda kategorileri tekrar yükleyip formu göster
            viewModel.Categories = await GetCategoriesSelectListAsync(viewModel.CategoryId);
            return View(viewModel);
        }


        // Yardımcı metot: Kategorileri SelectList olarak getirir
        private async Task<SelectList> GetCategoriesSelectListAsync(object? selectedValue = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            // DTO listesini SelectListItem listesine dönüştür
            var selectListItems = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            // Başa "-- Seçiniz --" ekle
            selectListItems.Insert(0, new SelectListItem { Value = "0", Text = "-- Kategori Seçiniz --" });

            return new SelectList(selectListItems, "Value", "Text", selectedValue);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id.Value <= 0) return NotFound();

            var postDto = await _blogPostService.GetBlogPostByIdAsync(id.Value);
            if (postDto == null) return NotFound();

            // Yetki Kontrolü
            var userId = _userManager.GetUserId(User);
            if (postDto.Author?.Id != userId && !User.IsInRole("Admin")) // Author DTO'da Id olmalı! UserDto'ya ekleyelim.
            {
                _logger.LogWarning("Unauthorized attempt to edit post {PostId} by user {UserId}", id, userId);
                return Forbid(); // Yetkisiz erişim
            }

            // GeneralProfile.cs içine şu map'lemeyi ekleyin: CreateMap<BlogPostDetailDto, BlogPostEditViewModel>();
            // VEYA manuel map'leme:
            var viewModel = _mapper.Map<BlogPostEditViewModel>(postDto); // DTO'yu ViewModel'a map'le
                                                                         // var viewModel = new BlogPostEditViewModel
                                                                         // {
                                                                         //     Id = postDto.Id,
                                                                         //     Title = postDto.Title,
                                                                         //     Content = postDto.Content,
                                                                         //     CategoryId = postDto.Category?.Id ?? 0, // Category null olabilir
                                                                         //     ImageUrl = postDto.ImageUrl
                                                                         // };

            viewModel.Categories = await GetCategoriesSelectListAsync(viewModel.CategoryId); // Seçili kategoriyi ayarla

            return View(viewModel);
        }

        // POST: BlogPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPostEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest(); // ID eşleşmiyorsa hatalı istek

            // Yetki Kontrolü (Tekrar!)
            var postToCheck = await _blogPostService.GetBlogPostByIdAsync(id); // Sadece ID ve UserID çekmek daha verimli olabilir
            if (postToCheck == null) return NotFound();
            var currentUserId = _userManager.GetUserId(User);
            if (postToCheck.Author?.Id != currentUserId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("Unauthorized POST attempt to edit post {PostId} by user {UserId}", id, currentUserId);
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ViewModel'dan Update DTO'ya map'le
                    // GeneralProfile.cs içine şu map'lemeyi ekleyin: CreateMap<BlogPostEditViewModel, BlogPostUpdateDto>();
                    var updateDto = _mapper.Map<BlogPostUpdateDto>(viewModel);

                    bool success = await _blogPostService.UpdateBlogPostAsync(updateDto, currentUserId); // Serviste de yetki kontrolü yapılıyor

                    if (success)
                    {
                        _logger.LogInformation("Blog post {PostId} updated successfully by user {UserId}", id, currentUserId);
                        TempData["SuccessMessage"] = "Blog yazısı başarıyla güncellendi!";
                        return RedirectToAction("Details", new { id = viewModel.Id });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Blog yazısı güncellenirken bir hata oluştu veya yetkiniz yok.");
                        _logger.LogError("Blog post update failed for post {PostId} by user {UserId}.", id, currentUserId);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Beklenmedik bir hata oluştu.");
                    _logger.LogError(ex, "Exception occurred during blog post update for post {PostId} by user {UserId}.", id, currentUserId);
                }
            }
            else
            {
                _logger.LogWarning("Edit POST called with invalid model state for post {PostId}.", id);
            }

            // Hata veya geçersiz model durumunda formu tekrar göster
            viewModel.Categories = await GetCategoriesSelectListAsync(viewModel.CategoryId);
            return View(viewModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id.Value <= 0) return NotFound();

            var postDto = await _blogPostService.GetBlogPostByIdAsync(id.Value); // Detayları göster
            if (postDto == null) return NotFound();

            // Yetki Kontrolü
            var userId = _userManager.GetUserId(User);
            if (postDto.Author?.Id != userId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("Unauthorized attempt to access delete confirmation for post {PostId} by user {UserId}", id, userId);
                return Forbid();
            }

            // DTO'yu View'a gönder (Detay göstermek için DetailDto iyi)
            return View(postDto);
        }

        [HttpPost, ActionName("Delete")] // Action adını Delete olarak belirtiyoruz
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postToCheck = await _blogPostService.GetBlogPostByIdAsync(id); // Sadece ID ve UserID çekmek daha verimli olabilir
            if (postToCheck == null) return NotFound(); // Post zaten yoksa veya hata varsa
            var currentUserId = _userManager.GetUserId(User);
            if (postToCheck.Author?.Id != currentUserId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("Unauthorized POST attempt to delete post {PostId} by user {UserId}", id, currentUserId);
                return Forbid();
            }

            try
            {
                bool success = await _blogPostService.DeleteBlogPostAsync(id, currentUserId); // Servis yetki kontrolü yapabilir

                if (success)
                {
                    _logger.LogInformation("Blog post {PostId} deleted successfully by user {UserId}", id, currentUserId);
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla silindi!";
                    return RedirectToAction("Index", "Home"); // Anasayfaya yönlendir
                }
                else
                {
                    // Hata durumunda kullanıcıyı bilgilendir (belki Delete sayfasına geri dönüp hata mesajı göster)
                    _logger.LogError("Blog post deletion failed for post {PostId} by user {UserId}.", id, currentUserId);
                    TempData["ErrorMessage"] = "Blog yazısı silinirken bir hata oluştu.";
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during blog post deletion for post {PostId} by user {UserId}.", id, currentUserId);
                TempData["ErrorMessage"] = "Blog yazısı silinirken beklenmedik bir hata oluştu.";
                return RedirectToAction("Delete", new { id = id });
            }
        }


        [AllowAnonymous] // Herkes kategorilere göre listeleyebilsin
        public async Task<IActionResult> Category(int? categoryId)
        {
            if (categoryId == null || categoryId.Value <= 0)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
            if (category == null)
            {
                return NotFound(); // Kategori yoksa
            }

            ViewBag.CategoryName = category.Name; // Kategori adını View'a gönder
            ViewData["Title"] = $"{category.Name} Kategorisi"; // Sayfa başlığını ayarla

            var blogPosts = await _blogPostService.GetBlogPostsByCategoryAsync(categoryId.Value);

            return View("~/Views/Home/Index.cshtml", blogPosts); // View yolunu belirt
                                                                 // Eğer Views/BlogPosts/Index.cshtml varsa (Home/Index'in kopyası/benzeri):
                                                                 // return View("Index", blogPosts);
        }

        [AllowAnonymous]
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
