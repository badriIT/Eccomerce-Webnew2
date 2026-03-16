using Eccomerce_Web.Modules.Auth.Service.Implementation;
using Eccomerce_Web.Modules.Auth.Service.Interfaces;

namespace Eccomerce_Web.Modules.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
