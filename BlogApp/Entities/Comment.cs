using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment has to be filled.")]
        [StringLength(1000, ErrorMessage = "Comment can be max 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        public DateTime CommentDate { get; set; } = DateTime.Now;

        // ilişki yapıları
        [Required(ErrorMessage = "Blog post has to be selected.")]
        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost? BlogPost { get; set; } = null!;

        [Required(ErrorMessage = "Author has to be selected.")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User? Author { get; set; } = null!;
    }
}
