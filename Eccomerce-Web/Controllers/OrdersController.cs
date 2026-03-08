using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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



        [HttpGet("Get-All-Orders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var orders = await _db.Orders     /// here 
                .Where(o => o.UserId == userId)
                .Include(o => o.Products)
                    .ThenInclude(ci => ci.Product).AsNoTracking()
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

            return Ok(new ApiResponse<List<OrderDto>>
            {
                Data = ordersDto,
                Status = StatusCodes.Status200OK,
                Message = "Orders retrieved successfully"
            });
        }


        [HttpPost("Post-Order")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> CreateSingleItemOrder(int Pid, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            if (Quantity <= 0)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Quantity must be greater than zero"
                });

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

            var orderitem = await _db.Orders.FirstOrDefaultAsync(o => o.UserId == userId &&
            o.Products.Any(oi => oi.ProductId == Pid));

            if (orderitem != null) Quantity++;

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<OrderItem>
             {

             new OrderItem

             {
                ProductId = Pid,
                Quantity = Quantity
             }

            }
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var createdOrder = await _db.Orders
                .Include(o => o.Products)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderDto = new OrderDto
            {
                Id = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                Products = createdOrder.Products.Select(oi => new CartItemsForOrderDto
                {
                    ChoosedProductId = oi.Id,
                    SelectedQuantity = oi.Quantity,
                    Product = new ForOrderProductsDto
                    {
                        Id = oi.Product.Id,
                        Name = oi.Product.Name,
                        Size = oi.Product.Size,
                        Price = oi.Product.Price,
                        Quantity = oi.Quantity,
                        Category = oi.Product.Category,
                        CreatedAt = oi.Product.CreatedAt,
                        Description = oi.Product.Description,

                    }
                }).ToList()
            };

            return Ok(new ApiResponse<OrderDto>
            {
                Data = null,
                Status = StatusCodes.Status201Created,
                Message = "Order created successfully"
            });
        }


        
        [HttpPost("Create-Order-From-Cart")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> CreateOrderFromCart()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var cartItems = await _db.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest(new ApiResponse<bool> { Data = false, Status = StatusCodes.Status400BadRequest, Message = "Cart is empty" });

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Product not found for cart item {item.Id}"
                    });

                if (item.Quantity <= 0)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Invalid quantity for {item.Product.Name}"
                    });

                if (item.Quantity > item.Product.Quantity)
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Not enough stock for {item.Product.Name}"
                    });
            }

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {


                order.Products.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);

            await _db.SaveChangesAsync();

            var createdOrder = await _db.Orders
                .Include(o => o.Products)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderDto = new OrderDto
            {
                Id = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                Products = createdOrder.Products.Select(oi => new CartItemsForOrderDto
                {
                    ChoosedProductId = oi.Id,
                    SelectedQuantity = oi.Quantity,
                    Product = new ForOrderProductsDto
                    {
                        Id = oi.Product.Id,
                        Name = oi.Product.Name,
                        Size = oi.Product.Size,
                        Price = oi.Product.Price,
                        Quantity = oi.Quantity,
                        Category = oi.Product.Category,
                        CreatedAt = oi.Product.CreatedAt,
                        Description = oi.Product.Description,
                        IsFavorited = false
                    }
                }).ToList()
            };

            return Ok(new ApiResponse<OrderDto>
            {
                Data = orderDto,
                Status = StatusCodes.Status200OK,
                Message = "Order created successfully"
            });
        }



        


        [HttpPut("Order-Update")]  // From Badri (need to fix it in create order from cart it is not working says 404 not found product.)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> OrderUpdate(int OrderId, int OrdersProductsId, int Quantity)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var order = await _db.Orders
                .Include(c => c.Products)
                    .ThenInclude(u => u.Product)
                .FirstOrDefaultAsync(c => c.Id == OrderId && c.UserId == userId);

            if (order == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Order not found"
                });

            var orderItem = order.Products.FirstOrDefault(u => u.Id == OrdersProductsId);

            if (orderItem == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found in order"
                });

            if (Quantity <= 0 || Quantity > orderItem.Product.Quantity)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid quantity"
                });

            orderItem.Quantity = Quantity;

            await _db.SaveChangesAsync();



            return Ok(new ApiResponse<Order>
            {
                Data = null,
                Status = StatusCodes.Status200OK,
                Message = "Order updated"
            });
        }


        [HttpDelete("Order-Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        public async Task<IActionResult> RemoveOrder(int OrderId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Error Finding User!"
                });

            var order = await _db.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == OrderId && o.UserId == userId);

            if (order == null)
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Order not found"
                });

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return Ok(new ApiResponse<Order>
            {
                Data = null,
                Status = StatusCodes.Status200OK,
                Message = "Order Deleted"
            });
        }



        //    [HttpPut("Order-Update")]
        //    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        //    public async Task<IActionResult> OrderUpdate(int OrderId, int OrdersProductsId, int Quantity)
        //    {
        //        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        //            return Unauthorized();

        //        // FIX 1: Use ThenInclude instead of .Select() inside Include
        //        var order = await _db.Orders
        //            .Include(c => c.Products)
        //                .ThenInclude(u => u.Product)
        //            .FirstOrDefaultAsync(c => c.Id == OrderId && c.UserId == userId);

        //        var OrdersProducts = order.Products.Select(p => p.Product.Id == OrdersProductsId);


        //        //var user = await _db.UserProfiles.FirstOrDefaultAsync(o => o.UserId == userId);




        //        if (order == null)
        //        {
        //            return NotFound(new ApiResponse<bool>
        //            {
        //                Data = false,
        //                Status = StatusCodes.Status404NotFound,
        //                Message = "Order not found"
        //            });
        //        }

        //        if (!OrdersProducts.Any())
        //        {
        //            return NotFound(new ApiResponse<bool>
        //            {
        //                Data = false,
        //                Status = StatusCodes.Status404NotFound,
        //                Message = "Products Id not found"
        //            });
        //        }

        //        if (order.Products.Any(u => u.Product.Quantity <= 0))
        //        {
        //            return BadRequest(new ApiResponse<bool>
        //            {
        //                Data = false,
        //                Status = StatusCodes.Status400BadRequest,
        //                Message = "Invalid quantity"
        //            });
        //        }

        //        //order.Products.ForEach(o => o.Quantity = Quantity);

        //        var updateOrderDto = new CartItemsForOrderDto()
        //        {
        //            SelectedQuantity = Quantity
        //        };


        //        await _db.SaveChangesAsync();

        //        return Ok(new ApiResponse<Order>
        //        {
        //            Data = order,
        //            Status = StatusCodes.Status200OK,
        //            Message = "Order updated"
        //        });
        //    }
        //}
    }
}


