using findspot_backend.Models;

namespace findspot_backend.Repository
{
    public interface IReviewRepository
    {
        void Add(Review review);
        void Update(Review review);
        void Delete(Guid reviewId);
        IEnumerable<Review> GetByBlogPostId(Guid blogPostId);
        Review GetById(Guid reviewId);
        public IEnumerable<Review> GetAllByUserId(string userId);
    }
}