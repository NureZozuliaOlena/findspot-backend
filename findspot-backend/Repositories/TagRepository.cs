using findspot_backend.Data;
using findspot_backend.Models;

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
    }
}