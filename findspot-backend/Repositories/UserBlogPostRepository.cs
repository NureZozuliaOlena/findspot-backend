using Microsoft.EntityFrameworkCore;
using findspot_backend.Data;
using findspot_backend.Models;
using findspot_backend.Models.DTO;

namespace findspot_backend.Repositories
{
    public class UserBlogPostRepository : IUserBlogPostRepository
    {
        private readonly FindSpotDbContext _context;

        public UserBlogPostRepository(FindSpotDbContext context)
        {
            _context = context;
        }

        public UserBlogPost Add(UserBlogPost userBlogPost)
        {
            _context.UserBlogPosts.Add(userBlogPost);
            _context.SaveChanges();

            return userBlogPost;
        }

        public UserBlogPost Delete(Guid blogId, string userId)
        {
            var userBlogPost = _context.UserBlogPosts
                .FirstOrDefault(ubp => ubp.UserId == userId && ubp.BlogPostId == blogId);

            if (userBlogPost != null)
            {
                _context.UserBlogPosts.Remove(userBlogPost);
                _context.SaveChanges();
            }

            return userBlogPost;
        }

        public IEnumerable<UserBlogPost> GetAllUserBlogs(string userId)
        {
            return _context.UserBlogPosts
                .Where(ubp => ubp.UserId == userId)
                .Include(ubp => ubp.BlogPost)
                .ToList();
        }

        public bool HasVisited(Guid blogId, string userId)
        {
            return _context.UserBlogPosts
                .Any(ubp => ubp.UserId == userId && ubp.BlogPostId == blogId && ubp.Status == "visited");
        }
    }
}