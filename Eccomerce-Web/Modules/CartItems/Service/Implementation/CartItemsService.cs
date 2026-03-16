using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models.Cart;
using Eccomerce_Web.Modules.CartItems.Dtos.Response;
using Eccomerce_Web.Modules.CartItems.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.CartItems.Service.Implementation
{
    public class CartItemsService : ICartItemsService
    {
        private readonly DataContext _db;

        public CartItemsService(DataContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<ForCartItems>>> GetCartItems(int userId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<List<ForCartItems>>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<List<ForCartItems>>.BadRequest("You need to verify your account first!");

            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

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
                    Description = c.Product.Description
                }
            }).ToList();

            double totalPrice = cartItemsDto.Sum(item => item.SelectedQuantity * item.Product.Price);

            return new ApiResponse<List<ForCartItems>>
            {
                Data = cartItemsDto,
                Status = 200,
                Message = $"Cart items retrieved successfully. Total price: {totalPrice}"
            };
        }

        public async Task<ApiResponse<bool>> AddToCart(int userId, int productId, int quantity)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return ApiResponse<bool>.NotFound("Product not found");

            if (quantity <= 0 || quantity > product.Quantity)
                return ApiResponse<bool>.BadRequest("Invalid quantity");

            var existingItem = _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId && c.ProductId == productId)
                .FirstOrDefault();

            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > product.Quantity)
                    return ApiResponse<bool>.BadRequest("Exceeds available stock");

                existingItem.Quantity += quantity;
                await _db.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Data = true,
                    Status = 200,
                    Message = "Cart quantity updated"
                };
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            _db.CartItems.Add(cartItem);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Successfully added"
            };
        }

        public async Task<ApiResponse<bool>> UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var cartItem = await _db.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
                return ApiResponse<bool>.NotFound("Cart item not found");

            if (quantity <= 0 || quantity > cartItem.Product.Quantity)
                return ApiResponse<bool>.BadRequest("Invalid quantity");

            cartItem.Quantity = quantity;
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Cart item updated"
            };
        }

        public async Task<ApiResponse<bool>> ClearCart(int userId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var cartItems = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();

            if (cartItems.Count == 0)
                return ApiResponse<bool>.NotFound("Cart is already empty");

            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Cart cleared successfully"
            };
        }

        public async Task<ApiResponse<bool>> RemoveFromCart(int userId, int cartItemId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            if (!user.isVerified)
                return ApiResponse<bool>.BadRequest("You need to verify your account first!");

            var cartItem = await _db.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
                return ApiResponse<bool>.NotFound("Product is not found in cart");

            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Successfully removed"
            };
        }
    }
}
