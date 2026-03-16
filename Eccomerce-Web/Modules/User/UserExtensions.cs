using Eccomerce_Web.Modules.User.Service.Implementation;
using Eccomerce_Web.Modules.User.Service.Interfaces;

namespace Eccomerce_Web.Modules.User
{
    public static class UserExtensions
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
           services.AddScoped<IUserService, UserService>();
           services.AddAuthorization(options =>
            {
               options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

         
            return services;
        }
    }
}
