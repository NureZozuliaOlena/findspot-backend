using System.ComponentModel.DataAnnotations.Schema;

namespace findspot_backend.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserId { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        [NotMapped]
        public string? UserName { get; set; }

        [NotMapped]
        public IFormFile? FeaturedImage { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}