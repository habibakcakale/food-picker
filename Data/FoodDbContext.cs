using FoodApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Data
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Meal>().ToTable(nameof(Meal)).HasKey(prop => prop.Id);
            modelBuilder.Entity<Order>().ToTable(nameof(Order)).HasKey(item => item.Id);
            modelBuilder.Entity<Order>().HasMany(order => order.OrderItems).WithOne();
            modelBuilder.Entity<OrderItem>().ToTable(nameof(OrderItem)).Property(item => item.Id).ValueGeneratedOnAdd();
        }
    }
}