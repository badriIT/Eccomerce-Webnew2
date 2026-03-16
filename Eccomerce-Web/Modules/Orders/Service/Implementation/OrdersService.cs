using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
using Eccomerce_Web.Modules.Orders.Dtos.Response;
using Eccomerce_Web.Modules.Orders.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.Orders.Service.Implementation
{
    public class OrdersService : IOrdersService
    {
        private readonly DataContext _db;

        public OrdersService(DataContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<OrderDto>>> GetAllOrders(int userId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<List<OrderDto>>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<List<OrderDto>>.BadRequest("You need to verify your account first!");

            var orders = await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Products)
                    .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .ToListAsync();

            if (!orders.Any())
                return new ApiResponse<List<OrderDto>>
                {
                    Data = new List<OrderDto>(),
                    Status = 200,
                    Message = "No orders found"
                };

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrdersEnums,
                Products = o.Products.Select(ci => new CartItemsForOrderDto
                {
                    ChoosedProductId = ci.Id,
                    SelectedQuantity = ci.Quantity,
                    Product = new ForOrderProductsDto
                    {
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        Price = ci.Product.Price,
                        Quantity = ci.Product.Quantity,
                        Size = ci.Product.Size,
                        Category = ci.Product.Category,
                        CreatedAt = ci.Product.CreatedAt
                    }
                }).ToList()
            }).ToList();

            return new ApiResponse<List<OrderDto>>
            {
                Data = ordersDto,
                Status = 200,
                Message = "Orders retrieved successfully"
            };
        }

        public async Task<ApiResponse<bool>> CreateSingleItemOrder(int userId, int productId, int quantity)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            if (quantity <= 0)
                return ApiResponse<bool>.BadRequest("Quantity must be greater than zero");

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return ApiResponse<bool>.NotFound("Product not found");

            if (quantity > product.Quantity)
                return ApiResponse<bool>.BadRequest("Not enough stock");

            var orderitem = await _db.Orders.FirstOrDefaultAsync(o => o.UserId == userId &&
                o.Products.Any(oi => oi.ProductId == productId));

            if (orderitem != null) quantity++;

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = productId,
                        Quantity = quantity
                    }
                }
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 201,
                Message = "Order created successfully"
            };
        }

        public async Task<ApiResponse<OrderDto>> CreateOrderFromCart(int userId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<OrderDto>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<OrderDto>.BadRequest("You need to verify your account first!");

            var cartItems = await _db.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return ApiResponse<OrderDto>.BadRequest("Cart is empty");

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                    return ApiResponse<OrderDto>.BadRequest($"Product not found for cart item {item.Id}");

                if (item.Quantity <= 0)
                    return ApiResponse<OrderDto>.BadRequest($"Invalid quantity for {item.Product.Name}");

                if (item.Quantity > item.Product.Quantity)
                    return ApiResponse<OrderDto>.BadRequest($"Not enough stock for {item.Product.Name}");
            }

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                order.Products.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            var createdOrder = await _db.Orders
                .Include(o => o.Products)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderDto = new OrderDto
            {
                Id = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                Products = createdOrder.Products.Select(oi => new CartItemsForOrderDto
                {
                    ChoosedProductId = oi.Id,
                    SelectedQuantity = oi.Quantity,
                    Product = new ForOrderProductsDto
                    {
                        Id = oi.Product.Id,
                        Name = oi.Product.Name,
                        Size = oi.Product.Size,
                        Price = oi.Product.Price,
                        Quantity = oi.Quantity,
                        Category = oi.Product.Category,
                        CreatedAt = oi.Product.CreatedAt,
                        Description = oi.Product.Description,
                        IsFavorited = false
                    }
                }).ToList()
            };

            return new ApiResponse<OrderDto>
            {
                Data = orderDto,
                Status = 200,
                Message = "Order created successfully"
            };
        }

        public async Task<ApiResponse<bool>> UpdateOrder(int userId, int orderId, int ordersProductsId, int quantity)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var order = await _db.Orders
                .Include(c => c.Products)
                    .ThenInclude(u => u.Product)
                .FirstOrDefaultAsync(c => c.Id == orderId && c.UserId == userId);

            if (order == null)
                return ApiResponse<bool>.NotFound("Order not found");

            var orderItem = order.Products.FirstOrDefault(u => u.Id == ordersProductsId);

            if (orderItem == null)
                return ApiResponse<bool>.NotFound("Product not found in order");

            if (quantity <= 0 || quantity > orderItem.Product.Quantity)
                return ApiResponse<bool>.BadRequest("Invalid quantity");

            orderItem.Quantity = quantity;
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Order updated"
            };
        }

        public async Task<ApiResponse<bool>> RemoveOrder(int userId, int orderId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var order = await _db.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return ApiResponse<bool>.NotFound("Order not found");

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Order Deleted"
            };
        }
    }
}
