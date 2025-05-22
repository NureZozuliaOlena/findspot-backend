using Microsoft.EntityFrameworkCore;
using findspot_backend.Data;
using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly FindSpotDbContext _dbContext;

        public BlogPostRepository(FindSpotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BlogPost Add(BlogPost blogPost)
        {
            _dbContext.BlogPosts.Add(blogPost);
            _dbContext.SaveChanges();

            return blogPost;
        }

        public bool Delete(Guid id)
        {
            var existingBlogPost = _dbContext.BlogPosts.Find(id);

            if (existingBlogPost != null)
            {
                _dbContext.BlogPosts.Remove(existingBlogPost);
                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public BlogPost Get(Guid id)
        {
            return _dbContext.BlogPosts
                .Include(bp => bp.Tags)
                .Include(bp => bp.Reviews)
                .Include(bp => bp.TouristObject)
                .Include(nameof(BlogPost.Tags))
                .FirstOrDefault(bp => bp.Id == id);
        }

        public IEnumerable<BlogPost> GetAll()
        {
            return _dbContext.BlogPosts
            .Include(nameof(BlogPost.Tags))
            .ToList();
        }

        public BlogPost Update(BlogPost blogPost)
        {
            var existingBlogPost = _dbContext.BlogPosts.Find(blogPost.Id);

            if (existingBlogPost != null)
            {
                existingBlogPost.PageTitle = blogPost.PageTitle;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.ShortDescription = blogPost.ShortDescription;
                existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlogPost.PublishedDate = blogPost.PublishedDate;
                existingBlogPost.UserId = blogPost.UserId;
                existingBlogPost.TouristObject = blogPost.TouristObject;

                if (blogPost.Tags != null && blogPost.Tags.Any())
                {
                    _dbContext.Tags.RemoveRange(existingBlogPost.Tags);

                    blogPost.Tags.ToList().ForEach(x => x.BlogPostId = existingBlogPost.Id);
                    _dbContext.Tags.AddRange(blogPost.Tags);
                }

                existingBlogPost.Reviews = blogPost.Reviews;

                _dbContext.SaveChanges();
            }

            return existingBlogPost;
        }

        public IEnumerable<TouristObject> GetAllTouristObjects()
        {
            return _dbContext.TouristObjects.ToList() ?? new List<TouristObject>();
        }

        public BlogPost GetByUrl(string urlHandle)
        {
            return _dbContext.BlogPosts
                .Include(bp => bp.Tags)
                .Include(bp => bp.Reviews)
                .Include(bp => bp.TouristObject)
                .Include(nameof(BlogPost.Tags))
                .FirstOrDefault();
        }
    }
}