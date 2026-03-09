using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Enums;
using Eccomerce_Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly DataContext _Db;

        public ProductsController(DataContext Db) => _Db = Db;






        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetProducts(
            Category? category,
            double? minPrice,
            double? maxPrice,
            string? sort,
            int page = 1,
            int pageSize = 10)
        {
            var query = _Db.Products.AsQueryable();

            // Filtering
            if (category.HasValue)
                query = query.Where(p => p.Category == category);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            // Sorting
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Pagination
            var products = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new ApiResponse<List<Product>>
            {
                Data = products,
                Status = StatusCodes.Status200OK,
                Message = "Products retrieved successfully"
            });
        }



        [HttpGet("get-product-by-id/{id}")]

        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _Db.Products.FindAsync(id);
            if (product == null)
            {
                ApiResponse<bool> response = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not Found"
                };

                return NotFound(response);
            }

            ApiResponse<Product> ApiRes = new()
            {
                Data = product,
                Status = StatusCodes.Status200OK,
                Message = ""
            };

            return Ok(ApiRes);

        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {

            if (product == null)
            {
                ApiResponse<bool> Response = new ApiResponse<bool>()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found"

                };

                return BadRequest(Response);
            }

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Models State is not valid!"
                });


            Product newProduct = new()
            {
                Name = product.Name,
                Size = product.Size,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                Description = product.Description,

            };

            ApiResponse<Product> response = new ApiResponse<Product>
            {

                Message = "Product added successfully",
                Data = newProduct,
                Status = StatusCodes.Status201Created
            };

            _Db.Products.Add(newProduct);
            await _Db.SaveChangesAsync();
            return Ok(response);

        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProductById(int id, ProductDto updatedProduct)
        {
            var product = await _Db.Products.FindAsync(id);
            if (product == null)
            {
                ApiResponse<bool> response = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not Found"
                };

                return NotFound(response);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            product.Name = updatedProduct.Name;
            product.Size = updatedProduct.Size;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            product.Category = updatedProduct.Category;
            product.Description = updatedProduct.Description;

            await _Db.SaveChangesAsync();

            ApiResponse<Product> ApiResOk = new()
            {
                Data = product,
                Status = StatusCodes.Status200OK,
                Message = "Product Updated"
            };

            return Ok(ApiResOk);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("update-product-quantity/{id}")]
        public async Task<IActionResult> UpdateProductsQuantity(int id, int Quantity)
        {
            var product = await _Db.Products.FindAsync(id);
            if (product == null)
            {

                ApiResponse<bool> response = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not Found"
                };

                return NotFound(response);
            }

            if (!ModelState.IsValid)

                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Model State is not Valid"
                });

            product.Quantity = Quantity;

            await _Db.SaveChangesAsync();

            ApiResponse<Product> ApiResOk = new()
            {
                Data = product,
                Status = StatusCodes.Status200OK,
                Message = "Product Updated"
            };

            return Ok(ApiResOk);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("delete-product/{id}")]

        public async Task<IActionResult> DeleteProductById(int id)
        {

            var product = await _Db.Products.FindAsync(id);

            if (product == null)
            {
                ApiResponse<bool> response = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not Found"
                };

                return NotFound(response);
            }

            _Db.Products.Remove(product);
            await _Db.SaveChangesAsync();
            return NoContent();

        }
    }



}





// return types when delete NoContent
// on post =>   return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct); like this 201 create returns
// NotFound
// validation write in dtos range - required  

//  if (!ModelState.IsValid) return BadRequest(ModelState) checks if info is hows in dto [required] [range] ;
//  if( something ==  null) write above the creating model

// USE AI TO BUILD AND LEARN BETTER !!!!!!!!!!!!!!!!!