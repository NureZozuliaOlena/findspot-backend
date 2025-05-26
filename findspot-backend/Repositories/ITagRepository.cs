using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAll();
        IEnumerable<BlogPost> GetPostsByTag(string tagName);
    }
}