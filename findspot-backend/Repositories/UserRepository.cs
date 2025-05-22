using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using findspot_backend.Data;
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

            DateTime lockoutEndDate;

            switch (banDuration)
            {
                case "Unban":
                    lockoutEndDate = DateTime.UtcNow;
                    break;
                case "Day":
                    lockoutEndDate = DateTime.UtcNow.AddDays(1);
                    break;
                case "Week":
                    lockoutEndDate = DateTime.UtcNow.AddDays(7);
                    break;
                case "Month":
                    lockoutEndDate = DateTime.UtcNow.AddMonths(1);
                    break;
                case "Year":
                    lockoutEndDate = DateTime.UtcNow.AddYears(1);
                    break;
                case "Forever":
                    lockoutEndDate = DateTime.UtcNow.AddYears(1000);
                    break;
                default:
                    lockoutEndDate = DateTime.UtcNow;
                    break;
            }
            await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return user;
        }
    }
}