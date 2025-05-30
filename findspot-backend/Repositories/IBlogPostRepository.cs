using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public interface IBlogPostRepository
    {
        BlogPost Add(BlogPost blogPost);
        IEnumerable<BlogPost> GetAll();
        BlogPost Get(Guid id);
        BlogPost GetByUrl(string urlHandle);
        BlogPost Update(BlogPost blogPost);
        bool Delete(Guid id);
        IEnumerable<TouristObject> GetAllTouristObjects();
        double? GetAverageRating(Guid blogPostId);
    }
}