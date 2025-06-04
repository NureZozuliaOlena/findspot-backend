using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleExists = await _userManager.IsInRoleAsync(user, StaticDetail.Role_User);
            if (!roleExists)
            {
                var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(StaticDetail.Role_User))
                {
                    await roleManager.CreateAsync(new IdentityRole(StaticDetail.Role_User));
                }

                await _userManager.AddToRoleAsync(user, StaticDetail.Role_User);
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt" });

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return Unauthorized(new { message = "Invalid login attempt" });

            await _signInManager.SignInAsync(user, isPersistent: false);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = "Logged in successfully",
                userId = user.Id,
                userName = user.UserName,
                email = user.Email,
                roles = roles
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out" });
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Ok(new { isLoggedIn = false });
                }
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    isLoggedIn = true,
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    roles = roles,
                    accountVerified = user.AccountVerified
                });
            }

            return Ok(new { isLoggedIn = false });
        }
    }
}
