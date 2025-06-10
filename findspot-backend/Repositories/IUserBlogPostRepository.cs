using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public interface IUserBlogPostRepository
    {
        public UserBlogPost Add(UserBlogPost userBlogPost);
        public UserBlogPost Delete(Guid blogId, string userId);
        IEnumerable<UserBlogPost> GetAllUserBlogs(string userId);
        bool HasVisited(Guid blogId, string userId);
        bool Exists(string userId, Guid blogPostId);
    }
}