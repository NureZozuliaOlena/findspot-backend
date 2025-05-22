using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TouristObjectsController : ControllerBase
    {
        private readonly ITouristObjectRepository _repository;
        private readonly IMapper _mapper;

        public TouristObjectsController(ITouristObjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var touristObjects = _repository.GetAll();
            var dtos = _mapper.Map<IEnumerable<TouristObjectDto>>(touristObjects);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var touristObject = _repository.Get(id);
            if (touristObject == null)
                return NotFound();

            var dto = _mapper.Map<TouristObjectDto>(touristObject);
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TouristObjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<TouristObject>(dto);
            entity.Id = Guid.NewGuid();

            var created = _repository.Add(entity);
            var createdDto = _mapper.Map<TouristObjectDto>(created);
            return CreatedAtAction(nameof(Get), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] TouristObjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var entity = _mapper.Map<TouristObject>(dto);
            var updated = _repository.Update(entity);

            if (updated == null)
                return NotFound();

            return Ok(_mapper.Map<TouristObjectDto>(updated));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var success = _repository.Delete(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
