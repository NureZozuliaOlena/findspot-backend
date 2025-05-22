using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace findspot_backend.Models
{
    public class User : IdentityUser
    {
        [NotMapped]
        public IFormFile? AvatarImage { get; set; }
        public string? AvatarImageUrl { get; set; }
        public bool AccountVerified { get; set; }
        public ICollection<UserBlogPost>? UserBlogPosts { get; set; }
    }
}