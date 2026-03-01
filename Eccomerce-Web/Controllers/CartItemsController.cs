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
    public class CartItemsController(DataContext db, IJWTService JW) : ControllerBase
    {

        private readonly DataContext _db = db;
        private readonly IJWTService _IJWTService = JW;
        
        

        [HttpPost("Cart-Product-Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> AddToCart(int ProductId, int Quantity)
        {
            var userClimes = User.FindFirst(ClaimTypes.NameIdentifier);
           
            if (userClimes == null) {
                return Unauthorized();
            }

            int userId = int.Parse(userClimes.Value);
            
            var FoundedUser = await _db.UserProfiles.Include(c => c.CartItems).FirstOrDefaultAsync(u => u.UserId == userId);

            if (FoundedUser == null) {
                return Unauthorized();
            }
            

            var FoundedProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == ProductId);

            if (FoundedProduct == null) {

                ApiResponse<bool> ProductNotFound = new ApiResponse<bool>
                {

                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found"
                };

                return NotFound(ProductNotFound);
            }

            if (Quantity <= 0 || Quantity > FoundedProduct.Quantity)
            {
                ApiResponse<bool> InvalidQuantity = new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid Quantity"
                };
                return BadRequest(InvalidQuantity);
            }


                CartItem NewCartItem = new()
                {
                    UserId = userId,
                    ProductId = ProductId,
                    Quantity = Quantity
                };


               FoundedUser.CartItems.Add(NewCartItem);






              _db.UserProfiles.Update(FoundedUser);
              _db.CartItems.Add(NewCartItem);
              await _db.SaveChangesAsync();


            ApiResponse<CartItem> response = new ApiResponse<CartItem>
            {

                Data = NewCartItem,
                Status = StatusCodes.Status200OK,
                Message = "Succsessfully added"
            };





            return Ok("Added to cart");
        }







    }
}
