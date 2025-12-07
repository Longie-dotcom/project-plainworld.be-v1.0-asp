using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace PlainWorld.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPrivilegeAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, PrivilegeHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPrivilegePolicyProvider>();

            return services;
        }
    }
}
