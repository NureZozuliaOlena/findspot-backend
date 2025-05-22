namespace findspot_backend.Models
{
    public class UserBlogPost
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
        public string Status { get; set; }
    }
}
