

using System.Security.Claims;
using Eccomerce_Web.Data;
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
    public class CartItemsController : ControllerBase
    {

        private readonly DataContext _db;
        private readonly IJWTService _IJWTService;

        public CartItemsController(DataContext db, IJWTService jw)
        {
            _db = db;
            _IJWTService = jw;
        }

        [HttpPost("Cart-Product-Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> AddToCart(int ProductId, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return Unauthorized();

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == ProductId);

            if (product == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found"
                });

            if (Quantity <= 0 || Quantity > product.Quantity)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid quantity"
                });

            // Check if product already exists in cart
            var existingItem = user.CartItems.FirstOrDefault(c => c.ProductId == ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += Quantity;
                await _db.SaveChangesAsync();

                var updatedItem = await _db.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == existingItem.Id);

                return Ok(new ApiResponse<CartItem>
                {
                    Data = updatedItem,
                    Status = StatusCodes.Status200OK,
                    Message = "Cart quantity updated"
                });
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = ProductId,
                Quantity = Quantity
            };

            user.CartItems.Add(cartItem);
            await _db.SaveChangesAsync();

            // Reload with product details
            var addedItem = await _db.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItem.Id);

            return Ok(new ApiResponse<CartItem>
            {
                Data = addedItem,
                Status = StatusCodes.Status200OK,
                Message = "Successfully added"
            });
        }





    }
}