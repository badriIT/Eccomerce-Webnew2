using BCrypt.Net;
using Eccomerce_Web.Enums;
using Eccomerce_Web.Models.Admin;
using Eccomerce_Web.Models.Product;

namespace Eccomerce_Web.Data
{
    public static class DbSeeder
    {
        public static void Seed(DataContext context)
        {
            if (context.Products.Any())
                return; // already seeded



            var products = new List<Product>
{


    new Product
    {
        Id = 1,
        Name = "Laptop",
        Size = "15.6 inch",
        Price = 999.99,
        Quantity = 10,
        Category = Category.Electronics,
        CreatedAt = DateTime.UtcNow,
        Description = "High performance laptop",
        IsFavorited = false
    },
    new Product
    {
        Id = 2,
        Name = "T-Shirt",
        Size = "M",
        Price = 19.99,
        Quantity = 50,
        Category = Category.Clothing,
        CreatedAt = DateTime.UtcNow,
        Description = "Cotton t-shirt",
        IsFavorited = false
    },
    new Product
    {
        Id = 3,
        Name = "Microwave",
        Size = "20L",
        Price = 120.00,
        Quantity = 15,
        Category = Category.HomeAppliances,
        CreatedAt = DateTime.UtcNow,
        Description = "Kitchen microwave oven",
        IsFavorited = false
    },
    new Product
    {
        Id = 4,
        Name = "Novel Book",
        Size = "Standard",
        Price = 9.99,
        Quantity = 100,
        Category = Category.Books,
        CreatedAt = DateTime.UtcNow,
        Description = "Fiction novel",
        IsFavorited = false
    },
    new Product
    {
        Id = 5,
        Name = "Toy Car",
        Size = "Small",
        Price = 14.99,
        Quantity = 80,
        Category = Category.Toys,
        CreatedAt = DateTime.UtcNow,
        Description = "Kids toy car",
        IsFavorited = false
    },
    new Product
    {
        Id = 6,
        Name = "Football",
        Size = "5",
        Price = 25.00,
        Quantity = 40,
        Category = Category.SportsEquipment,
        CreatedAt = DateTime.UtcNow,
        Description = "Professional football",
        IsFavorited = false
    },
    new Product
    {
        Id = 7,
        Name = "Shampoo",
        Size = "500ml",
        Price = 6.50,
        Quantity = 120,
        Category = Category.BeautyProducts,
        CreatedAt = DateTime.UtcNow,
        Description = "Hair care shampoo",
        IsFavorited = false
    },
    new Product
    {
        Id = 8,
        Name = "Car Oil",
        Size = "1L",
        Price = 18.99,
        Quantity = 60,
        Category = Category.Auto,
        CreatedAt = DateTime.UtcNow,
        Description = "Engine oil",
        IsFavorited = false
    },
    new Product
    {
        Id = 9,
        Name = "Office Chair",
        Size = "Standard",
        Price = 89.99,
        Quantity = 25,
        Category = Category.Furniture,
        CreatedAt = DateTime.UtcNow,
        Description = "Ergonomic chair",
        IsFavorited = false
    },
    new Product
    {
        Id = 10,
        Name = "Gold Ring",
        Size = "18",
        Price = 199.99,
        Quantity = 5,
        Category = Category.Jewelry,
        CreatedAt = DateTime.UtcNow,
        Description = "18k gold ring",
        IsFavorited = false
    }



};
            context.Products.AddRange(products);
            context.SaveChanges();


            if(context.Admins.Any())
                return; // already seeded

            var admin = new Admin
            {
               email = "admin@gmail.com",
               password = BCrypt.Net.BCrypt.HashPassword("Admin1234*"),
            };

            context.Admins.Add(admin);
            context.SaveChanges();
        }


    }

}
