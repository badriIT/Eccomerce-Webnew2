using FluentValidation.AspNetCore;

namespace Eccomerce_Web.Common.Extensions
{
    public static class FluentValidation
    {
        public static IServiceCollection AddFluentValidator(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}
