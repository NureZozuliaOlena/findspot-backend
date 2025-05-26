using System.Security.Claims;

namespace findspot_backend.Services
{
    public interface IAuthorizationHelperService
    {
        Task<(bool canEdit, bool canDelete)> CheckPermissionsAsync(string resourceOwnerId, ClaimsPrincipal user);
    }

}
