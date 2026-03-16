using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Enums;
using Eccomerce_Web.Modules.Admin.Services.Interfaces;
using Eccomerce_Web.Modules.Orders.Dtos.Response;
using Eccomerce_Web.Modules.User.Dtos.Response;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.Admin.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _db;

        public AdminService(DataContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<OrdersAdminDto>> GetAllSuccessfulOrders()
        {
            var orders = await _db.Orders
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .Where(n => n.OrdersEnums == OrdersEnums.succseed)
                .AsNoTracking()
                .ToListAsync();

            if (!orders.Any())
                return ApiResponse<OrdersAdminDto>.NotFound("Orders not found");

            var totalOrders = orders.Count;

            var totalRevenue = orders
                .SelectMany(o => o.Products)
                .Sum(p => p.Quantity * p.Product.Price);

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

            return new ApiResponse<OrdersAdminDto>
            {
                Status = 200,
                Message = "Whole Orders received",
                Data = new OrdersAdminDto
                {
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    Orders = ordersDto
                }
            };
        }

        public async Task<ApiResponse<List<OnlyUserInfoDto>>> GetAllUsers()
        {
            var users = await _db.UserProfiles
                .Select(o => new OnlyUserInfoDto
                {
                    Email = o.Email,
                    FullName = o.FullName,
                    PhoneNumber = o.PhoneNumber,
                    Id = o.UserId,
                    Role = o.Role
                })
                .AsNoTracking()
                .ToListAsync();

            if (!users.Any())
                return ApiResponse<List<OnlyUserInfoDto>>.NotFound("Users not found");

            return new ApiResponse<List<OnlyUserInfoDto>>
            {
                Status = 200,
                Message = "All users loaded",
                Data = users
            };
        }
    }
}
