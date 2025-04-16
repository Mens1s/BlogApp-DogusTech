using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class CommentCreateDto
    {
        [Required(ErrorMessage = "Yorum içeriği zorunludur.")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int BlogPostId { get; set; }
    }
}
