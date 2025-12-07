using Application.Helper;
using Application.Interface.IService;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;

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
            // Auto mapper
            services.AddAutoMapper(cfg => { }, typeof(Mapper).Assembly);

            // Services
            services.AddScoped<IPlayerService, PlayerService>();

            // gRPC Service

            return services;
        }
        #endregion
    }
}
