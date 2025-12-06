using Application.Interface;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application
{
    public static class ApplicationModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IEmailApplication, EmailApplication>();

            // gRPC Service

            return services;

        }
        #endregion
    }
}
