using API.APIException;
using System.Security.Claims;

namespace API.Helper
{
    public static class CheckClaimHelper
    {
        public static (string userId, string role) CheckClaim(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new ClaimNotFound("User ID not found in claims.");
            }

            var role = user.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(role))
            {
                throw new ClaimNotFound("Role not found in claims.");
            }

            return (userId, role);
        }
    }
}
