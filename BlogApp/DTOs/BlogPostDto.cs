namespace BlogApp.DTOs
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
