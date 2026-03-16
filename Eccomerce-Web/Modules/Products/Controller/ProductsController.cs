using Eccomerce_Web.Enums;
using Eccomerce_Web.Modules.Products.Dtos.Request;
using Eccomerce_Web.Modules.Products.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eccomerce_Web.Modules.Products.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }


        [HttpGet("Get-All-Categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var res = await _productsService.GetAllCategories();
            return StatusCode(res.Status, res);
        }


        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetProducts(
            Category? category,
            double? minPrice,
            double? maxPrice,
            string? sort,
            int page = 1,
            int pageSize = 10)
        {
            var res = await _productsService.GetProducts(category, minPrice, maxPrice, sort, page, pageSize);
            return StatusCode(res.Status, res);
        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var res = await _productsService.GetProductById(id);
            return StatusCode(res.Status, res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _productsService.AddProduct(product);
            return StatusCode(res.Status, res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProductById(int id, ProductDto updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _productsService.UpdateProductById(id, updatedProduct);
            return StatusCode(res.Status, res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("update-product-quantity/{id}")]
        public async Task<IActionResult> UpdateProductsQuantity(int id, int Quantity)
        {
            var res = await _productsService.UpdateProductQuantity(id, Quantity);
            return StatusCode(res.Status, res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var res = await _productsService.DeleteProductById(id);
            return StatusCode(res.Status, res);
        }


       
    }   
}
