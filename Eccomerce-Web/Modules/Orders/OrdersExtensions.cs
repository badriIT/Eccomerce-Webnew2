using Eccomerce_Web.Modules.Orders.Service.Implementation;
using Eccomerce_Web.Modules.Orders.Service.Interfaces;

namespace Eccomerce_Web.Modules.Orders
{
    public static class OrderExtensions
    {
        public static IServiceCollection AddOrdersModule(this IServiceCollection services)
        {
            services.AddScoped<IOrdersService, OrdersService>();
            return services;
        }
    }
}
