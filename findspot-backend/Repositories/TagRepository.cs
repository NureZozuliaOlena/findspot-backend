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

        public IEnumerable<BlogPost> GetPostsByTag(string tagName)
        {
            return _dbContext.BlogPosts
                .Include(bp => bp.Tags)
                .Where(bp => bp.Tags.Any(t => t.Name.ToLower() == tagName.ToLower()))
                .ToList();
        }
    }
}