namespace findspot_backend.Models.DTO
{
    public class ReviewCreateDto
    {
        public string Content { get; set; }
        public int Rating { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
