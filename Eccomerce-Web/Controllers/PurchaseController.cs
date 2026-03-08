using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly IJWTService _IJWTService;

        public PurchaseController(DataContext db, IJWTService jw)
        {
            _db = db;
            _IJWTService = jw;
        }

        [HttpPost("Buy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> Buy(string Onumber)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == Onumber);

            if (order == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Order not found"
                });

            if(order.OrdersEnums == Enums.OrdersEnums.succseed)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Order is already paid"
                });
            }

            if (order.UserId != userId)
                return Forbid();

            var orderItems = await _db.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .Include(oi => oi.Product)
                .ToListAsync();

            if (!orderItems.Any())
                return NotFound(new ApiResponse<Order>
                {
                    Data = order,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Order has no products"
                });

            foreach (var item in orderItems)
            {
                if (item.Product == null)
                    return NotFound(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Product not found for order item {item.Id}"
                    });

                if (item.Product.Quantity < item.Quantity)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Not enough stock for {item.Product.Name}"
                    });

                item.Product.Quantity -= item.Quantity;
            }
            order.OrdersEnums = Enums.OrdersEnums.succseed;
            await _db.SaveChangesAsync();

            return Ok(new ApiResponse<Order>
            {
                Data = order,
                Status = StatusCodes.Status200OK,
                Message = "Product Bought Successfully"
            });
        }
        }
    }

