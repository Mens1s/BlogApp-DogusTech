using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.ViewModels.BlogPost
{
    public class BlogPostEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Görsel URL'si (Opsiyonel)")]
        [StringLength(500)]
        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }

        public SelectList? Categories { get; set; }
    }
}
