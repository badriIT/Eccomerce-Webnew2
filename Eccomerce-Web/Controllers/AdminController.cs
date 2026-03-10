using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly IJWTService _JWTService;

        public AdminController(DataContext db, IJWTService jwt)
        {
            _db = db;
            _JWTService = jwt;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Get-All-Succsessfull-Orders")]
        public async Task<IActionResult> GetAllSecsessfullOrders()
        {
            var Orders = await _db.Orders.Include(o => o.Products).ThenInclude(p => p.Product).Where(n => n.OrdersEnums == Enums.OrdersEnums.succseed).AsNoTracking().ToListAsync();

            var totalOrders = Orders.Count;

            var totalRevenue = Orders
                .SelectMany(o => o.Products)
                .Sum(p => p.Quantity * p.Product.Price);

            if (!Orders.Any())
            {


                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Orders not found"
                });
            }


            var ordersDto = Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrdersEnums,
                Products = o.Products.Select(ci => new CartItemsForOrderDto
                {
                    ChoosedProductId = ci.Id,
                    SelectedQuantity = ci.Quantity,           //////////// like tthis
                    Product = new ForOrderProductsDto
                    {
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        Price = ci.Product.Price,
                        Quantity = ci.Product.Quantity,
                        Size = ci.Product.Size,
                        Category = ci.Product.Category,
                        CreatedAt = ci.Product.CreatedAt,


                    }





                }).ToList()

            }).ToList();

            return Ok(
     new ApiResponse<OrdersAdminDto>
     {
         Data = new OrdersAdminDto
         {
             TotalOrders = totalOrders,
             TotalRevenue = totalRevenue,
             Orders = ordersDto
         },
         Status = StatusCodes.Status200OK,
         Message = "Whole Orders received"
     }
 );
        }









        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var AllUsers = await _db.UserProfiles
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

            if (!AllUsers.Any())
            {
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Users not found"
                });
            }

            return Ok(new ApiResponse<List<OnlyUserInfoDto>>
            {
                Data = AllUsers,
                Status = StatusCodes.Status200OK,
                Message = "All users loaded"
            });
        }

















    }

}
