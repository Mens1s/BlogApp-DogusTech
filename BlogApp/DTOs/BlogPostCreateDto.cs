using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class BlogPostCreateDto
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
    }
}
