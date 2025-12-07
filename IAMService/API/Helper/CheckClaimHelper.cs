using API.APIException;
using System.Security.Claims;

namespace API.Helper
{
    public static class CheckClaimHelper
    {
        public static (Guid userId, string role) CheckClaim(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new ClaimNotFound("User ID not found in claims.");
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new ClaimNotFound("User ID in claims is not a valid GUID.");
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
