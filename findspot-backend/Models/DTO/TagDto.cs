namespace findspot_backend.Models.DTO
{
    public class TagDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? BlogPostId { get; set; }
    }
}
