using Eccomerce_Web.Models.Admin;
using Eccomerce_Web.Models.Cart;
using Eccomerce_Web.Models.Product;
using Eccomerce_Web.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Data
{
    public class DataContext : DbContext
    {
       
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Admin> Admins { get; set; }


       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=dpg-d7d4njm7r5hc739sge0g-a.oregon-postgres.render.com;Database=eccomerce_db_11kj;Username=eccomerce_db_11kj_user;Password=lCMENU2QIkIZLO7UUzGDVr6w3Yhwi1wN");
            }
        }
    }
}
