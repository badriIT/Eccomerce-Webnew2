using Eccomerce_Web.Modules.CartItems.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eccomerce_Web.Modules.CartItems.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;

        public CartItemsController(ICartItemsService cartItemsService)
        {
            _cartItemsService = cartItemsService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet("Cart-Products")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> GetCartItems()
        {
            var res = await _cartItemsService.GetCartItems(GetUserId());
            return StatusCode(res.Status, res);
        }

        [HttpPost("Cart-Product-Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> AddToCart(int ProductId, int Quantity)
        {
            var res = await _cartItemsService.AddToCart(GetUserId(), ProductId, Quantity);
            return StatusCode(res.Status, res);
        }

        [HttpPut("Cart-Product-Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> UpdateCartItem(int CartItemId, int Quantity)
        {
            var res = await _cartItemsService.UpdateCartItem(GetUserId(), CartItemId, Quantity);
            return StatusCode(res.Status, res);
        }

        [HttpDelete("Cart-Product-Clear-whole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> ClearCart()
        {
            var res = await _cartItemsService.ClearCart(GetUserId());
            return StatusCode(res.Status, res);
        }

        [HttpDelete("Cart-Product-Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> RemoveFromCart(int CartItemId)
        {
            var res = await _cartItemsService.RemoveFromCart(GetUserId(), CartItemId);
            return StatusCode(res.Status, res);
        }
    }
}
