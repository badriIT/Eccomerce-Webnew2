using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
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
            // 1️⃣ Get user id from token
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            // 2️⃣ Validate quantity
            if (Quantity <= 0)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Quantity must be greater than zero"
                });

            // 3️⃣ Check product
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == Pid);

            if (product == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found"
                });

            // 4️⃣ Check stock
            if (Quantity > product.Quantity)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Not enough stock"
                });

            // 5️⃣ Create new order (ALWAYS NEW)
            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<CartItem>
        {
            new CartItem
            {
                UserId = userId,
                ProductId = Pid,
                Quantity = Quantity
            }
        }
            };

            // 6️⃣ Save directly to Orders table
            _db.Orders.Add(order); // and like here update 00:17 04.03.2026 shit

            await _db.SaveChangesAsync();

            // 7️⃣ Reload with product info (optional but clean)
            var createdOrder = await _db.Orders
                .Include(o => o.Products)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return Ok(new ApiResponse<Order>
            {
                Data = createdOrder,
                Status = StatusCodes.Status200OK,
                Message = "Order created successfully"
            });
        }


        [HttpGet("Get-All-Orders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> GetAllOrders()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var orders = await _db.Orders     /// here 
                .Where(o => o.UserId == userId)
                .Include(o => o.Products)
                    .ThenInclude(ci => ci.Product)
                .ToListAsync();

            if (!orders.Any())
                return Ok(new ApiResponse<List<OrderDto>>
                {
                    Data = new List<OrderDto>(),
                    Status = StatusCodes.Status200OK,
                    Message = "No orders found"
                });

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Products = o.Products.Select(ci => new CartItemsForOrderDto
                {
                    CartItemIdInCart = ci.Id,
                    SelectedQuantity = ci.Quantity,           //////////// like tthis
                    Product = new ForOrderProductsDto
                    {
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        Price = ci.Product.Price,
                        Quantity = ci.Quantity
                    }
                }).ToList()
            }).ToList();

            return Ok(new ApiResponse<List<OrderDto>>
            {
                Data = ordersDto,
                Status = StatusCodes.Status200OK,
                Message = "Orders retrieved successfully"
            });
        }
        [HttpPost("Create-Order-From-Cart")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> CreateOrderFromCart()   ///// only this is to make work form _db finding with user iD I GOT TO GO LAST TIME 01:02 04.03.2026 thx 
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userP = await _db.UserProfiles
                .Include(u => u.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(u => u.UserId == userId);


            if (userP == null)
                return Unauthorized();

            if (userP.CartItems == null || !userP.CartItems.Any())
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Cart is empty"
                });

            foreach (var cartItem in userP.CartItems)
            {
                if (cartItem.Product == null)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Product not found for cart item {cartItem.Id}"
                    });

                if (cartItem.Quantity <= 0)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Invalid quantity for product: {cartItem.Product.Name}"
                    });









            }





            Order order = new Order
            {
                UserId = userP.UserId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = userP.CartItems.Select(ci => new CartItem
                {
                    UserId = userP.UserId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity
                }).ToList()
            };


            _db.CartItems.RemoveRange(userP.CartItems); // Remove cart items from the database
            userP.CartItems.Clear(); // Clear the cart after creating the order
            _db.Orders.Add(order); // Add the order to the user's orders





            ////var order = new Order
            ////{
            ////    UserId = userP.Id,
            ////    OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
            ////    Products = userP.CartItems.ToList(),
            ////    User = userP.User,
            ////};

            //var order = new OrderDto
            //{

            //    OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
            //    Products = new()
            //    {
            //        new CartItemsForOrderDto()
            //        {
            //            Product = new()
            //            {
            //                ForOrderProductsDto  /// amas vasworeb da sen gaagrzele me unda wavideeeeeeee!!!!!!!!!!!!!!!!!!!
            //                {

            //                }
            //            }
            //        }
            //    },
            //    User = userP.User,
            //};

            //userP.Order ??= new List<Order>(); // es ar wasalo imitomaa erori rom zemot var order dastrixulia
            //userP.Order.Add(order);

            //// ??= <-- es aris 
            ////if (variable == null)
            ////{
            ////    variable = value;
            ////} ase amowmebs ra


            await _db.SaveChangesAsync();

            var AddedOrders = await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Products)
                    .ThenInclude(ci => ci.Product)
                .ToListAsync();

            if (AddedOrders == null || !AddedOrders.Any())
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "No orders found after creation"
                });



            return Ok(new ApiResponse<List<Order>>
            {
                Data = AddedOrders, // ← return the re-fetched entity, not the local variable
                Status = StatusCodes.Status200OK,
                Message = "Order created successfully"
            });
        }



        [HttpDelete("Order-Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> RemoveOrder(int OrderId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();



            //bool userExists = await _db.UserProfiles.AnyAsync(u => u.UserId == userId);
            //if (!userExists) return Unauthorized();f

            var order = await _db.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.Id == OrderId);

            if (order == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Order not found"
                });

            if (order.UserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You do not have permission to delete this order"
                });

            if (order.Products != null && order.Products.Any())
                _db.CartItems.RemoveRange(order.Products);

            _db.Orders.Remove(order);
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


