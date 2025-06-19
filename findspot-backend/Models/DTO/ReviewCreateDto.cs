using System.ComponentModel.DataAnnotations;

namespace findspot_backend.Models.DTO
{
    public class ReviewCreateDto
    {
        [MaxLength(1500, ErrorMessage = "Максимальна довжина коментаря — 1500 символів.")]
        public string Content { get; set; }
        public int Rating { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
