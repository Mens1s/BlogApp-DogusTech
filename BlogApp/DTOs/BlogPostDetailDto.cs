namespace BlogApp.DTOs
{
    public class BlogPostDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string? ImageUrl { get; set; }

        public UserDto? Author { get; set; } 
        public CategoryDto? Category { get; set; } 

        public IList<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
