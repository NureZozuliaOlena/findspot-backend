using AutoMapper;
using findspot_backend.Models;
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

        [HttpGet]
        public IActionResult GetAllTags()
        {
            var tags = _tagRepository.GetAll();
            var tagDtos = _mapper.Map<List<TagDto>>(tags);
            return Ok(tagDtos);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetTagById(Guid id)
        {
            var tag = _tagRepository.GetById(id);
            if (tag == null)
                return NotFound();

            var tagDto = _mapper.Map<TagDto>(tag);
            return Ok(tagDto);
        }

        [HttpPost]
        public IActionResult CreateTag([FromBody] TagDto tagDto)
        {
            if (tagDto == null || string.IsNullOrWhiteSpace(tagDto.Name))
                return BadRequest("Tag name is required.");

            var existingTag = _tagRepository.GetByName(tagDto.Name);
            if (existingTag != null)
                return Conflict($"Tag with name '{tagDto.Name}' already exists.");

            var tag = _mapper.Map<Tag>(tagDto);
            tag.Id = Guid.NewGuid();

            _tagRepository.Add(tag);
            var createdTagDto = _mapper.Map<TagDto>(tag);

            return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, createdTagDto);
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTag(Guid id, [FromBody] TagDto tagDto)
        {
            if (tagDto == null || string.IsNullOrWhiteSpace(tagDto.Name))
                return BadRequest("Tag name is required.");

            var existingTag = _tagRepository.GetById(id);
            if (existingTag == null)
                return NotFound();

            existingTag.Name = tagDto.Name;

            _tagRepository.Update(existingTag);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteTag(Guid id)
        {
            var existingTag = _tagRepository.GetById(id);
            if (existingTag == null)
                return NotFound();

            _tagRepository.Delete(id);

            return NoContent();
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
