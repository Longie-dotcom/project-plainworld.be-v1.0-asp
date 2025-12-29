using SignalR.SignalRException;
using System.Security.Claims;

namespace SignalR.Helper
{
    public static class JwtClaimHelper
    {
        public static PlayerIdentity Extract(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                throw new ClaimNotFound("User is not authenticated.");

            // User ID
            var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdRaw))
                throw new ClaimNotFound("User ID not found in token.");

            if (!Guid.TryParse(userIdRaw, out var userId))
                throw new ClaimNotFound("User ID is not a valid GUID.");

            // Role
            var role = user.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(role))
                throw new ClaimNotFound("Role not found in token.");

            // Player name (priority order)
            var fullName =
                user.FindFirstValue("FullName") ??
                user.FindFirstValue(ClaimTypes.Name) ??
                user.FindFirstValue(ClaimTypes.Email) ??
                "Unknown";

            return new PlayerIdentity(
                userId,
                fullName,
                role
            );
        }
    }

    public readonly struct PlayerIdentity
    {
        public Guid UserId { get; }
        public string Name { get; }
        public string Role { get; }

        public PlayerIdentity(Guid userId, string name, string role)
        {
            UserId = userId;
            Name = name;
            Role = role;
        }
    }
}
