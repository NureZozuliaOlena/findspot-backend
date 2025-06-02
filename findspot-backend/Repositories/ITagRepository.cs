using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAll();
        Tag GetById(Guid id);
        Tag GetByName(string tagName);
        void Add(Tag tag);
        void Update(Tag tag);
        void Delete(Guid id);
        IEnumerable<BlogPost> GetPostsByTag(string tagName);
    }
}