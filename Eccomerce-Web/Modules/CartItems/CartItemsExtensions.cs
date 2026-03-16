using Eccomerce_Web.Modules.CartItems.Service.Implementation;
using Eccomerce_Web.Modules.CartItems.Service.Interfaces;

namespace Eccomerce_Web.Modules.CartItems
{
    public static class CartItemExtensions
    {
        public static IServiceCollection AddCartItemsModule(this IServiceCollection services)
        {
            services.AddScoped<ICartItemsService, CartItemsService>();
            return services;
        }
    }
}
