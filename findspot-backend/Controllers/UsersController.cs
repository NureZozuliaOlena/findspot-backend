using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace findspot_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserDto userDto)
        {
            var existingUser = await _userRepository.GetAsync(id);
            if (existingUser == null)
                return NotFound();

            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            var updatedUser = await _userRepository.UpdateAsync(user);
            if (updatedUser == null)
                return NotFound();

            return Ok(_mapper.Map<UserDto>(updatedUser));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("lock-unlock/{id}")]
        public async Task<IActionResult> LockUnlock(string id, [FromQuery] string duration)
        {
            var user = await _userRepository.LockUnlock(id, duration);
            if (user == null)
                return NotFound();

            return Ok(new { message = $"User lockout updated to: {duration}" });
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userRepository.GetUserRolesAsync(user);
            return Ok(roles);
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AddToRoles(string id, [FromBody] List<string> roles)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userRepository.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpDelete("{id}/roles")]
        public async Task<IActionResult> RemoveFromRoles(string id, [FromBody] List<string> roles)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userRepository.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}
