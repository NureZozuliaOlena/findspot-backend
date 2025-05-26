using findspot_backend.Models;
using findspot_backend.Utility;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace findspot_backend.Services
{
    public class AuthorizationHelperService : IAuthorizationHelperService
    {
        private readonly UserManager<User> _userManager;

        public AuthorizationHelperService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool canEdit, bool canDelete)> CheckPermissionsAsync(string resourceOwnerId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
                return (false, false);

            var isOwner = currentUser.Id == resourceOwnerId;
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, StaticDetail.Role_Admin);
            var isModerator = await _userManager.IsInRoleAsync(currentUser, StaticDetail.Role_Moderator);

            return (
                canEdit: isOwner,
                canDelete: isOwner || isAdmin || isModerator
            );
        }
    }

}
