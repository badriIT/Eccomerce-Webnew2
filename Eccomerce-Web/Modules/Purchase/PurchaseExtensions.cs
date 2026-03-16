using Eccomerce_Web.Modules.Purchase.Service.Implementation;
using Eccomerce_Web.Modules.Purchase.Service.Interfaces;

namespace Eccomerce_Web.Modules.Purchase
{
    public static class PurchaseExtensions
    {
        public static IServiceCollection AddPurchaseModule(this IServiceCollection services)
        {
            services.AddScoped<IPurchaseService, PurchaseService>();
            return services;
        }
    }
}
