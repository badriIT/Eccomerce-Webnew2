using Eccomerce_Web.Common.Extensions;
using Eccomerce_Web.Modules.Admin;
using Eccomerce_Web.Modules.Auth;
using Eccomerce_Web.Modules.CartItems;
using Eccomerce_Web.Modules.Orders;
using Eccomerce_Web.Modules.Products;
using Eccomerce_Web.Modules.Purchase;
using Eccomerce_Web.Modules.User;

namespace Eccomerce_Web.Extensions;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services)
    {
        services.AddUserModule();
        services.AddAuthModule();
        services.AddCartItemsModule();
        services.AddOrdersModule();
        services.AddProductsModule();
        services.AddPurchaseModule();
        services.AddAdminModule();
        services.AddRateLimiter();

        return services;
    }
}