using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Data;
using Eccomerce_Web.Enums;
using Eccomerce_Web.Models.Product;
using Eccomerce_Web.Modules.Products.Dtos.Request;
using Eccomerce_Web.Modules.Products.Dtos.Response;
using Eccomerce_Web.Modules.Products.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.Products.Service.Implementation
{
    public class ProductsService : IProductsService
    {
        private readonly DataContext _db;

        public ProductsService(DataContext db)
        {
            _db = db;
        }


        public Task<ApiResponse<List<string>>> GetAllCategories()
        {
            var categories = Enum.GetNames(typeof(Category)).ToList();

            var response = new ApiResponse<List<string>>
            {
                Data = categories,
                Status = 200,
                Message = "Categories retrieved successfully"
            };

            return Task.FromResult(response);
        }
        public async Task<ApiResponse<List<Product>>> GetProducts(
            Category? category, double? minPrice, double? maxPrice,
            string? sort, int page, int pageSize)
        {
            var query = _db.Products.AsQueryable();

            if (category.HasValue)
                query = query.Where(p => p.Category == category);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };


            var products = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new ProductsResponse<List<Product>>
            {
               
                Data = products,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Status = 200,
                Message = "Products retrieved successfully",
                
            };
        }

        public async Task<ApiResponse<Product>> GetProductById(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return ApiResponse<Product>.NotFound("Product not found");

            return new ApiResponse<Product>
            {
                Data = product,
                Status = 200,
                Message = ""
            };
        }

        public async Task<ApiResponse<Product>> AddProduct(ProductDto productDto)
        {
            var newProduct = new Product
            {
                Name = productDto.Name,
                Size = productDto.Size,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Category = productDto.Category,
                CreatedAt = productDto.CreatedAt,
                Description = productDto.Description
            };

            _db.Products.Add(newProduct);
            await _db.SaveChangesAsync();

            return ApiResponse<Product>.Created(newProduct, "Product added successfully");
        }

        public async Task<ApiResponse<Product>> UpdateProductById(int id, ProductDto updatedProduct)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return ApiResponse<Product>.NotFound("Product not found");

            product.Name = updatedProduct.Name;
            product.Size = updatedProduct.Size;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            product.Category = updatedProduct.Category;
            product.Description = updatedProduct.Description;

            await _db.SaveChangesAsync();

            return new ApiResponse<Product>
            {
                Data = product,
                Status = 200,
                Message = "Product Updated"
            };
        }

        public async Task<ApiResponse<Product>> UpdateProductQuantity(int id, int quantity)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return ApiResponse<Product>.NotFound("Product not found");

            product.Quantity = quantity;
            await _db.SaveChangesAsync();

            return new ApiResponse<Product>
            {
                Data = product,
                Status = 200,
                Message = "Product Updated"
            };
        }

        public async Task<ApiResponse<bool>> DeleteProductById(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return ApiResponse<bool>.NotFound("Product not found");

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 204,
                Message = "Product deleted"
            };
        }
    }
}
