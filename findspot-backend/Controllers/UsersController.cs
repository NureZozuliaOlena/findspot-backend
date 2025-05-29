using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using findspot_backend.Utility;
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

        [Authorize(Roles = $"{StaticDetail.Role_Admin}")]
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

        [Authorize(Roles = $"{StaticDetail.Role_Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = $"{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        [HttpPost("lock-unlock/{id}")]
        public async Task<IActionResult> LockUnlock(string id, [FromQuery] string duration)
        {
            var user = await _userRepository.LockUnlock(id, duration);
            if (user == null)
                return NotFound();

            return Ok(new { message = $"User lockout updated to: {duration}" });
        }

        [Authorize(Roles = $"{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        [HttpPut("{id}/verify")]
        public async Task<IActionResult> SetVerificationStatus(string id, [FromQuery] bool value)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return NotFound();

            user.AccountVerified = value;

            var updatedUser = await _userRepository.UpdateAsync(user);
            if (updatedUser == null)
                return StatusCode(500, "Unable to update verification status.");

            return Ok(new { message = $"User verification set to: {value}" });
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

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] UserDto userDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var existingUser = await _userRepository.GetAsync(userId);
            if (existingUser == null)
                return NotFound();

            existingUser.UserName = userDto.UserName;
            existingUser.Email = userDto.Email;
            existingUser.AvatarImageUrl = userDto.AvatarImageUrl;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            if (updatedUser == null)
                return StatusCode(500, "Unable to update profile.");

            return Ok(_mapper.Map<UserDto>(updatedUser));
        }

        [Authorize]
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest(new { message = "New password and confirmation do not match." });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return Unauthorized();

            var result = await _userRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password changed successfully." });
        }

    }
}
