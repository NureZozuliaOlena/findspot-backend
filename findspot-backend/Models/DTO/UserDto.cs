using System.Text.Json.Serialization;

namespace findspot_backend.Models.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? AvatarImageUrl { get; set; }
        public bool AccountVerified { get; set; }
        public ICollection<UserBlogPostDto>? UserBlogPosts { get; set; }
    }
}
