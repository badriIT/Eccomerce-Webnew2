using Eccomerce_Web.Common.Services.ServiceModels;
using Eccomerce_Web.Modules.Auth.Service.Implementation;
using Eccomerce_Web.Modules.Auth.Service.Interfaces;

namespace Eccomerce_Web.Modules.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<EmailSettings>();
            return services;
        }
    }
}
