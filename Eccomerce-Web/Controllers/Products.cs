using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
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
     


        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {


            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


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

            _Db.Products.Add(newProduct);
            await _Db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);

        }



        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _Db.Products.ToListAsync();
            return Ok(products);


        }



        [HttpGet("get-product-by-id/{id}")]

        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _Db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);

        }


        [HttpDelete("delete-product/{id}")]

        public async Task<IActionResult> DeleteProductById(int id)
        {

            var product = await _Db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _Db.Products.Remove(product);
            await _Db.SaveChangesAsync();
            return NoContent();


        }

        [HttpPut("update-product/{id}")]

        public async Task<IActionResult> UpdateProductById(int id, ProductDto updatedProduct)
        {
            var product = await _Db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
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
            return Ok(product);
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