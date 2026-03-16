using Eccomerce_Web.Modules.Products.Service.Implementation;
using Eccomerce_Web.Modules.Products.Service.Interfaces;

namespace Eccomerce_Web.Modules.Products
{
    public static class ProductExtensions
    {
        public static IServiceCollection AddProductsModule(this IServiceCollection services)
        {
            services.AddScoped<IProductsService, ProductsService>();
            return services;
        }
    }
}
