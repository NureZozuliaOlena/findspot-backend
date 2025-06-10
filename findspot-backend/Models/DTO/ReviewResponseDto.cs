namespace findspot_backend.Models.DTO
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserName { get; set; }
        public string? UserId { get; set; }
        public BlogPostSummaryDto BlogPost { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
