using findspot_backend.Data;
using findspot_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace findspot_backend.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FindSpotDbContext _dbContext;

        public TagRepository(FindSpotDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public IEnumerable<Tag> GetAll()
        {
            return _dbContext.Tags.ToList();
        }

        public Tag GetById(Guid id)
        {
            return _dbContext.Tags.Find(id);
        }

        public Tag GetByName(string tagName)
        {
            return _dbContext.Tags.FirstOrDefault(t => t.Name == tagName);
        }

        public void Add(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            _dbContext.SaveChanges();
        }

        public void Update(Tag tag)
        {
            _dbContext.Tags.Update(tag);
            _dbContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var tag = _dbContext.Tags.Find(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<BlogPost> GetPostsByTag(string tagName)
        {
            return _dbContext.BlogPosts
                .Include(bp => bp.Tags)
                .Where(bp => bp.Tags.Any(t => t.Name.ToLower() == tagName.ToLower()))
                .ToList();
        }
    }
}