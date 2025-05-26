using AutoMapper;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagsController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet("{tagName}/posts")]
        public IActionResult GetPostsByTag(string tagName)
        {
            var posts = _tagRepository.GetPostsByTag(tagName);
            if (!posts.Any())
            {
                return NotFound($"No posts found with tag '{tagName}'.");
            }

            var postDtos = _mapper.Map<List<BlogPostDto>>(posts);
            return Ok(postDtos);
        }
    }
}
