namespace BlogApp.DTOs
{
    public class CommentsDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CommentDate { get; set; }
        public int BlogPostId { get; set; }
        public UserDto? Author { get; set; }
    }
}
