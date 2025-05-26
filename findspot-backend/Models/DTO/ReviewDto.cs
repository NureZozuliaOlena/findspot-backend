using System.Text.Json.Serialization;

namespace findspot_backend.Models.DTO
{
    public class ReviewDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserId { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPostDto? BlogPost { get; set; }
        public string? UserName { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
