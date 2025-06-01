namespace findspot_backend.Models.DTO
{
    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? AvatarImageUrl { get; set; }
        public bool AccountVerified { get; set; }
    }
}
