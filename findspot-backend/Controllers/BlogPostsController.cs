using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using findspot_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMapper _mapper;
        private readonly IAuthorizationHelperService _authHelper;

        public BlogPostsController(
            IBlogPostRepository blogPostRepository,
            IMapper mapper,
            IAuthorizationHelperService authHelper)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
            _authHelper = authHelper;
        }

        [HttpGet("tourist-objects")]
        public IActionResult GetTouristObjects()
        {
            var touristObjects = _blogPostRepository.GetAllTouristObjects();
            var touristObjectDtos = _mapper.Map<List<TouristObjectDto>>(touristObjects);
            return Ok(touristObjectDtos);
        }

        [HttpGet]
        public IActionResult GetAllBlogPosts()
        {
            var blogPosts = _blogPostRepository.GetAll();
            var blogPostDtos = _mapper.Map<List<BlogPostDto>>(blogPosts);

            foreach (var dto in blogPostDtos)
            {
                dto.AverageRating = _blogPostRepository.GetAverageRating(dto.Id);
            }

            return Ok(blogPostDtos);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetBlogPost(Guid id)
        {
            var blogPost = _blogPostRepository.Get(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var blogPostDto = _mapper.Map<BlogPostDto>(blogPost);
            blogPostDto.AverageRating = _blogPostRepository.GetAverageRating(blogPost.Id);

            return Ok(blogPostDto);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] BlogPostDto blogPostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized("Cannot determine user ID.");

            var blogPost = _mapper.Map<BlogPost>(blogPostDto);
            blogPost.Id = Guid.NewGuid();
            blogPost.UserId = userId;

            if (blogPost.TouristObjectId.HasValue)
            {
                var allObjects = _blogPostRepository.GetAllTouristObjects();
                if (!allObjects.Any(to => to.Id == blogPost.TouristObjectId))
                {
                    return NotFound($"TouristObject with ID {blogPost.TouristObjectId} not found.");
                }
            }

            if (blogPostDto.Tags != null && blogPostDto.Tags.Any())
            {
                blogPost.Tags = blogPostDto.Tags.Select(dto => new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    BlogPostId = blogPost.Id
                }).ToList();
            }

            var createdPost = _blogPostRepository.Add(blogPost);
            var resultDto = _mapper.Map<BlogPostDto>(createdPost);

            return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.Id }, resultDto);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] BlogPostDto blogPostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPost = _blogPostRepository.Get(id);
            if (existingPost == null)
                return NotFound();

            var (canEdit, _) = await _authHelper.CheckPermissionsAsync(existingPost.UserId.ToString(), User);
            if (!canEdit)
                return Forbid("You are not allowed to edit this post.");

            var updatedPost = _mapper.Map<BlogPost>(blogPostDto);

            updatedPost.Id = id;
            updatedPost.UserId = existingPost.UserId;

            if (updatedPost.TouristObjectId.HasValue)
            {
                var allObjects = _blogPostRepository.GetAllTouristObjects();
                if (!allObjects.Any(to => to.Id == updatedPost.TouristObjectId))
                    return NotFound($"TouristObject with ID {updatedPost.TouristObjectId} not found.");
            }

            var savedPost = _blogPostRepository.Update(updatedPost);
            var savedDto = _mapper.Map<BlogPostDto>(savedPost);

            return Ok(savedDto);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingPost = _blogPostRepository.Get(id);
            if (existingPost == null)
                return NotFound();

            var (_, canDelete) = await _authHelper.CheckPermissionsAsync(existingPost.UserId.ToString(), User);
            if (!canDelete)
                return Forbid("You are not allowed to delete this post.");

            var deleted = _blogPostRepository.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
