namespace findspot_backend.Models.DTO
{
    public class UserBlogPostDto
    {
        public string UserId { get; set; }
        public UserDto? User { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPostSummaryDto? BlogPost { get; set; }
        public string Status { get; set; }
    }
}
