using System.ComponentModel.DataAnnotations;

namespace findspot_backend.Models.DTO
{
    public class ReviewDto
    {
        public Guid Id { get; set; }

        [MaxLength(1500)]
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
