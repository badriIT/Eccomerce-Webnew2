using Eccomerce_Web.Modules.Orders.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eccomerce_Web.Modules.Orders.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet("Get-All-Orders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var res = await _ordersService.GetAllOrders(GetUserId());
            return StatusCode(res.Status, res);
        }

        [HttpPost("Post-Order")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> CreateSingleItemOrder(int Pid, int Quantity)
        {
            var res = await _ordersService.CreateSingleItemOrder(GetUserId(), Pid, Quantity);
            return StatusCode(res.Status, res);
        }

        [HttpPost("Create-Order-From-Cart")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> CreateOrderFromCart()
        {
            var res = await _ordersService.CreateOrderFromCart(GetUserId());
            return StatusCode(res.Status, res);
        }

        [HttpPut("Order-Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> OrderUpdate(int OrderId, int OrdersProductsId, int Quantity)
        {
            var res = await _ordersService.UpdateOrder(GetUserId(), OrderId, OrdersProductsId, Quantity);
            return StatusCode(res.Status, res);
        }

        [HttpDelete("Order-Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> RemoveOrder(int OrderId)
        {
            var res = await _ordersService.RemoveOrder(GetUserId(), OrderId);
            return StatusCode(res.Status, res);
        }
    }
}
