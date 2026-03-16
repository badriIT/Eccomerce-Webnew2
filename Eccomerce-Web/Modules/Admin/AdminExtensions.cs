using Eccomerce_Web.Modules.Admin.Services.Implementations;
using Eccomerce_Web.Modules.Admin.Services.Interfaces;

namespace Eccomerce_Web.Modules.Admin
{
    public static class AdminExtensions
    {
        public static IServiceCollection AddAdminModule(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            return services;
        }
    }
}
