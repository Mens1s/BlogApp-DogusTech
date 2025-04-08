using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities
{
    public class User : IdentityUser
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        // virtual keyword indicates that this property can be overridden in derived classes
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
