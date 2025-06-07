using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
        
        public async Task<User> GetAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(to => to.Id == userId);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }

            return false;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.NormalizedUserName = user.NormalizedUserName;
                existingUser.Email = user.Email;
                existingUser.NormalizedEmail = user.NormalizedEmail;
                existingUser.AvatarImageUrl = user.AvatarImageUrl;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.AccountVerified = user.AccountVerified;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return existingUser;
                }
            }

            return null;
        }
        
        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<User> LockUnlock(string userId, string banDuration)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            DateTimeOffset? lockoutEndDate;

            switch (banDuration)
            {
                case "Unban":
                    lockoutEndDate = null;
                    break;
                case "Day":
                    lockoutEndDate = DateTimeOffset.UtcNow.AddDays(1);
                    break;
                case "Week":
                    lockoutEndDate = DateTimeOffset.UtcNow.AddDays(7);
                    break;
                case "Month":
                    lockoutEndDate = DateTimeOffset.UtcNow.AddMonths(1);
                    break;
                case "Year":
                    lockoutEndDate = DateTimeOffset.UtcNow.AddYears(1);
                    break;
                case "Forever":
                    lockoutEndDate = DateTimeOffset.UtcNow.AddYears(1000);
                    break;
                default:
                    lockoutEndDate = null;
                    break;
            }

            await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return user;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
    }
}