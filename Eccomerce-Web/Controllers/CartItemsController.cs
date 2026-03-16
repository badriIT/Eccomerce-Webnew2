

using System.Security.Claims;
using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Models.Cart;

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

        [HttpGet("Cart-Products")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]

        public async Task<IActionResult> GetCartItems()
        {

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

           
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || userRoleClaim == null)
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "user Id Claim and user Role Claim is null"/// badri aq naxe sworia???????????????????? // from B KI!!!
                });

            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(u => u.UserId == userId);
       
            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId).AsNoTracking()
                .ToListAsync();


            if (user == null) return Unauthorized(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "User is null"
            });

            if (user.isVerified == false)
            {
                return
               BadRequest(
                   new ApiResponse<bool>
                   {
                       Data = false,
                       Status = StatusCodes.Status400BadRequest,
                       Message = "You need to verify Your account first!"

                   }
                   );
            }

            var cartItemsDto = cartItems.Select(c => new ForCartItems
            {
                CartItemIdInCart = c.Id,
                SelectedQuantity = c.Quantity,
                Product = new ForProfileProductDto
                {
                    Id = c.Product.Id,
                    Name = c.Product.Name,
                    Size = c.Product.Size,
                    Price = c.Product.Price,
                    Quantity = c.Product.Quantity,
                    Category = c.Product.Category,
                    CreatedAt = c.Product.CreatedAt,
                    Description = c.Product.Description,

                }
            }).ToList();

            double totalPrice = cartItemsDto.Sum(item => item.SelectedQuantity * item.Product.Price);

            return Ok(new ApiResponse<List<ForCartItems>>
            {
                Data = cartItemsDto,
                Status = StatusCodes.Status200OK,
                Message = $"Cart items retrieved successfully total price: {totalPrice}    {user.isVerified}",
            });
        }

        [HttpPost("Cart-Product-Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> AddToCart(int ProductId, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return Unauthorized(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Error Finding User!"
            });

            if (user.isVerified == false)
            {
                return
               BadRequest(
                   new ApiResponse<bool>
                   {
                       Data = false,
                       Status = StatusCodes.Status400BadRequest,
                       Message = "You need to verify Your account first!"

                   }
                   );
            }

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

            var existingItem = _db.CartItems.Include(c => c.Product).Where(c => c.UserId == userId && c.ProductId == ProductId).FirstOrDefault();


            if (existingItem != null)
            {
                if (existingItem.Quantity + Quantity > product.Quantity)
                    return BadRequest(new ApiResponse<bool>               
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Exceeds available stock"
                    });
                existingItem.Quantity += Quantity;
                await _db.SaveChangesAsync();

                var updatedItem = await _db.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == existingItem.Id);

                return Ok(new ApiResponse<CartItem>
                {
                    Data = null,
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

            _db.CartItems.Add(cartItem);
            await _db.SaveChangesAsync();

            // Reload with product details
            var addedItem = await _db.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItem.Id);

            return Ok(new ApiResponse<CartItem>
            {
                Data = null,
                Status = StatusCodes.Status200OK,
                Message = "Successfully added"
            });
        }

        [HttpPut("Cart-Product-Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> UpdateCartItem(int CartItemId, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });
           
            var user = await _db.UserProfiles
               .Include(u => u.CartItems)
               .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return Unauthorized(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Error Finding User!"
            });

            if (user.isVerified == false)
            {
                return
               BadRequest(
                   new ApiResponse<bool>
                   {
                       Data = false,
                       Status = StatusCodes.Status400BadRequest,
                       Message = "You need to verify Your account first!"

                   }
                   );
            }

            var cartItem = await _db.CartItems
               .Include(c => c.Product)
               .FirstOrDefaultAsync(c => c.Id == CartItemId && c.UserId == userId);



            if (cartItem == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Cart item not found"
                });
            if (Quantity <= 0 || Quantity > cartItem.Product.Quantity)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid quantity"
                });
            cartItem.Quantity = Quantity;                          //// working update also quantity check is working also
            await _db.SaveChangesAsync();

            var updatedItem = await _db.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == CartItemId);
            return Ok(new ApiResponse<CartItem>
            {
                Data = null,
                Status = StatusCodes.Status200OK,
                Message = "Cart item updated"
            });
        }

        [HttpDelete("Cart-Product-Clear-whole")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> ClearCart()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var user = await _db.UserProfiles
             .Include(u => u.CartItems)
             .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return Unauthorized(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Error Finding User!"
            });

            if (user.isVerified == false)
            {
                return
               BadRequest(
                   new ApiResponse<bool>
                   {
                       Data = false,
                       Status = StatusCodes.Status400BadRequest,
                       Message = "You need to verify Your account first!"

                   }
                   );
            }

            var cartItems = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
            if (cartItems.Count == 0)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Cart is already empty"
                });
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();
            return Ok(new ApiResponse<bool>
            {
                Data = true,
                Status = StatusCodes.Status200OK,
                Message = "Cart cleared successfully"
            });

        }

        [HttpDelete("Cart-Product-Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")] 


        public async Task<IActionResult> RemoveFromCart(int CartItemId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var user = await _db.UserProfiles
             .Include(u => u.CartItems)
             .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return Unauthorized(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Error Finding User!"
            });

            if (user.isVerified == false)
            {
                return
               BadRequest(
                   new ApiResponse<bool>
                   {
                       Data = false,
                       Status = StatusCodes.Status400BadRequest,
                       Message = "You need to verify Your account first!"

                   }
                   );
            }

          var cartItem = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == CartItemId && c.UserId == userId);
            if (cartItem == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product is not found in cart"
                });
            _db.CartItems.Remove(cartItem);

            await _db.SaveChangesAsync();
            return Ok(new ApiResponse<bool>
            {
                Data = true,
                Status = StatusCodes.Status200OK,
                Message = "Successfully removed"
            });
        }
    }


}




