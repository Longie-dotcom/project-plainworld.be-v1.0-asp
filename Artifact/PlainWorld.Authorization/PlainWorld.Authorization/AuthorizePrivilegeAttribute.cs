using Microsoft.AspNetCore.Authorization;

namespace PlainWorld.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizePrivilegeAttribute : AuthorizeAttribute
    {
        public AuthorizePrivilegeAttribute(string privilege)
        {
            Policy = privilege;
        }
    }
}
