using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
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

            if (Quantity <= 0)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Quantity must be greater than zero"
                });

            // Instead of loading the full user graph just to check existence:
            bool userExists = await _db.UserProfiles.AnyAsync(u => u.Id == userId);
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

            product.Quantity -= Quantity;
            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                Products = new List<CartItem>
            {
                new CartItem
                {
                    UserId = userId,  // ← still missing this
                    ProductId = Pid,
                    Quantity = Quantity
                }
            }
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var addedItem = await _db.Orders
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == order.Id);

            return Ok(new ApiResponse<Order>
            {
                Data = addedItem,  // ← return addedItem, not order
                Status = StatusCodes.Status200OK,
                Message = "Order created successfully"
            });
        }
    }
}
