using findspot_backend.Models;
using findspot_backend.Models.DTO;
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
        public IActionResult AddToList([FromBody] UserBlogPostCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId != dto.UserId)
                return Forbid();

            var exists = _repository.Exists(dto.UserId, dto.BlogPostId);
            if (exists)
                return Conflict(new { message = "This blog post is already in the user's list." });

            var model = new UserBlogPost
            {
                UserId = dto.UserId,
                BlogPostId = dto.BlogPostId,
                Status = dto.Status
            };

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

            var userBlogPosts = _repository.GetAllUserBlogs(userId);

            var dtoList = userBlogPosts.Select(ubp => new UserBlogPostDto
            {
                UserId = ubp.UserId,
                Status = ubp.Status,
                BlogPostId = ubp.BlogPostId,
                BlogPost = new BlogPostSummaryDto
                {
                    Id = ubp.BlogPost.Id,
                    PageTitle = ubp.BlogPost.PageTitle,
                    ShortDescription = ubp.BlogPost.ShortDescription,
                    FeaturedImageUrl = ubp.BlogPost.FeaturedImageUrl,
                    PublishedDate = ubp.BlogPost.PublishedDate,
                    TouristObjectTitle = ubp.BlogPost.TouristObject?.Name
                }
            }).ToList();

            return Ok(dtoList);
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
