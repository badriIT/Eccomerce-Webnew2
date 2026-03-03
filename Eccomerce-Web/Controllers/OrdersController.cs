using Eccomerce_Web.Data;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensibility;
using System.Security.Claims;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly IJWTService _JWTService;

        public OrderController(DataContext db, IJWTService jwt)
        {
            _db = db;
            _JWTService = jwt;
        }




        [HttpPost("Post-Order")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> CreateSingleItemOrder(int Pid, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            if (Quantity <= 0) // ← validate quantity before hitting the database + need validate Item quantity here
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Quantity must be greater than zero"
                });

            // Instead of loading the full user graph just to check existence:
            bool userExists = await _db.UserProfiles.AnyAsync(u => u.UserId == userId);
             if (!userExists) return Unauthorized();


             

            //var user = await _db.UserProfiles
            //    .Include(u => u.Order)
            //    .ThenInclude(p => p.Products)
            //    .FirstOrDefaultAsync(u => u.Id == userId);

            //if (user == null) return Unauthorized();

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == Pid);
            if (product == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found"
                });

            if (Quantity > product.Quantity)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Not enough stock"
                });

            //product.Quantity -= Quantity; <---- not here should be when buying the order not when creating it
            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<CartItem> // HERE IS ALSO SMALL ERROR
            {
                new CartItem
                {
                    UserId = userId,  // ← still missing this  //HERE
                    ProductId = Pid,
                    Quantity = Quantity
                }
            }
            };

            _db.Orders.Add(order); // <--- error it must be added to users Orders not to the Orders table directly because of the relationship dd
            await _db.SaveChangesAsync();

            var addedItem = await _db.Orders
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == order.Id);

            return Ok(new ApiResponse<Order>
            {
                Data = order,  // ← return addedItem, not order
                Status = StatusCodes.Status200OK,
                Message = "Order created successfully"
            });
        }
    }
}


