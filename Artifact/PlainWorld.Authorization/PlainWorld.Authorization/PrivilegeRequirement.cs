using Microsoft.AspNetCore.Authorization;

namespace PlainWorld.Authorization
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Privilege { get; }

        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }
    }
}
