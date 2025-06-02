namespace findspot_backend.Models.DTO
{
    public class UserBlogPostCreateDto
    {
        public string UserId { get; set; }
        public Guid BlogPostId { get; set; }
        public string Status { get; set; }
    }
}
