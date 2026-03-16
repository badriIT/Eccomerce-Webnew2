using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Data;
using Eccomerce_Web.Modules.Purchase.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.Purchase.Service.Implementation
{
    public class PurchaseService : IPurchaseService
    {
        private readonly DataContext _db;

        public PurchaseService(DataContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Buy(int userId, string orderNumber)
        {
            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return ApiResponse<bool>.NotFound("Order not found");

            if (order.OrdersEnums == Enums.OrdersEnums.succseed)
                return ApiResponse<bool>.BadRequest("Order is already paid");

            if (order.UserId != userId)
                return ApiResponse<bool>.Forbidden("Access denied");

            var orderItems = await _db.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .Include(oi => oi.Product)
                .ToListAsync();

            if (!orderItems.Any())
                return ApiResponse<bool>.BadRequest("Order has no products");

            foreach (var item in orderItems)
            {
                if (item.Product == null)
                    return ApiResponse<bool>.BadRequest($"Product not found for order item {item.Id}");

                if (item.Product.Quantity < item.Quantity)
                    return ApiResponse<bool>.BadRequest($"Not enough stock for {item.Product.Name}");

                item.Product.Quantity -= item.Quantity;
            }

            order.OrdersEnums = Enums.OrdersEnums.succseed;
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Product Bought Successfully"
            };
        }
    }
}
