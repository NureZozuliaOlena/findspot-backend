using findspot_backend.Data;
using findspot_backend.Models;

namespace findspot_backend.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly FindSpotDbContext findSpotdbDbContext;

        public ReviewRepository(FindSpotDbContext findSpotdbDbContext)
        {
            this.findSpotdbDbContext = findSpotdbDbContext;
        }

        public void Add(Review review)
        {
            findSpotdbDbContext.Reviews.Add(review);
            findSpotdbDbContext.SaveChanges();
        }

        public void Update(Review review)
        {
            findSpotdbDbContext.Reviews.Update(review);
            findSpotdbDbContext.SaveChanges();
        }

        public void Delete(Guid reviewId)
        {
            var review = findSpotdbDbContext.Reviews.Find(reviewId);
            if (review != null)
            {
                findSpotdbDbContext.Reviews.Remove(review);
                findSpotdbDbContext.SaveChanges();
            }
        }

        public IEnumerable<Review> GetByBlogPostId(Guid blogPostId)
        {
            return findSpotdbDbContext.Reviews.Where(r => r.BlogPostId == blogPostId).ToList();
        }

        public Review GetById(Guid reviewId)
        {
            return findSpotdbDbContext.Reviews.Find(reviewId);
        }

        public IEnumerable<Review> GetAllByUserId(string userId)
        {
            return findSpotdbDbContext.Reviews.Where(r => r.UserId == userId).ToList();
        }
    }
}