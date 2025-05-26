using findspot_backend.Models;
using findspot_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace findspot_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserBlogPostController : ControllerBase
    {
        private readonly IUserBlogPostRepository _repository;

        public UserBlogPostController(IUserBlogPostRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("add")]
        public IActionResult AddToList([FromBody] UserBlogPost model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId != model.UserId)
                return Forbid();

            var added = _repository.Add(model);
            return Ok(added);
        }

        [HttpDelete("remove/{blogPostId}")]
        public IActionResult RemoveFromList(Guid blogPostId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Forbid();

            var removed = _repository.Delete(blogPostId, userId);
            if (removed == null)
                return NotFound("Item not found.");

            return Ok(removed);
        }

        [HttpGet("my")]
        public IActionResult GetMyBlogPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Forbid();

            var list = _repository.GetAllUserBlogs(userId);
            return Ok(list);
        }

        [HttpGet("visited/{blogPostId}")]
        public IActionResult HasVisited(Guid blogPostId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Forbid();

            var visited = _repository.HasVisited(blogPostId, userId);
            return Ok(visited);
        }
    }
}
