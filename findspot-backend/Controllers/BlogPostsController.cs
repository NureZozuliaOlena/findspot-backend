using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMapper _mapper;

        public BlogPostsController(IBlogPostRepository blogPostRepository, IMapper mapper)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
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
            return Ok(blogPostDto);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] BlogPostDto blogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogPost = _mapper.Map<BlogPost>(blogPostDto);

            if (blogPost.TouristObjectId.HasValue)
            {
                var allObjects = _blogPostRepository.GetAllTouristObjects();
                if (!allObjects.Any(to => to.Id == blogPost.TouristObjectId))
                {
                    return NotFound($"TouristObject with ID {blogPost.TouristObjectId} not found.");
                }
            }

            var createdPost = _blogPostRepository.Add(blogPost);
            var resultDto = _mapper.Map<BlogPostDto>(createdPost);

            return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.Id }, resultDto);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public IActionResult Update(Guid id, [FromBody] BlogPostDto blogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blogPostDto.Id)
            {
                return BadRequest("Mismatched blog post ID.");
            }

            var existingPost = _blogPostRepository.Get(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var updatedPost = _mapper.Map<BlogPost>(blogPostDto);

            if (updatedPost.TouristObjectId.HasValue)
            {
                var allObjects = _blogPostRepository.GetAllTouristObjects();
                if (!allObjects.Any(to => to.Id == updatedPost.TouristObjectId))
                {
                    return NotFound($"TouristObject with ID {updatedPost.TouristObjectId} not found.");
                }
            }

            var savedPost = _blogPostRepository.Update(updatedPost);
            var savedDto = _mapper.Map<BlogPostDto>(savedPost);

            return Ok(savedDto);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            var deleted = _blogPostRepository.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
