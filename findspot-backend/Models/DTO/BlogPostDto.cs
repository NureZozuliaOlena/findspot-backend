using System.Text.Json.Serialization;

namespace findspot_backend.Models.DTO
{
    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public ICollection<TagDto>? Tags { get; set; }

        [JsonIgnore]
        public ICollection<ReviewDto>? Reviews { get; set; }
        public Guid? TouristObjectId { get; set; }
        
        [JsonIgnore]
        public ICollection<UserBlogPostDto>? UserBlogPosts { get; set; }
    }
}
