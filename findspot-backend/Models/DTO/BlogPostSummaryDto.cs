namespace findspot_backend.Models.DTO
{
    public class BlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string PageTitle { get; set; }
        public string ShortDescription { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? TouristObjectTitle { get; set; }
    }
}
