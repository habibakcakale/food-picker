using Meal.Models;
using Microsoft.EntityFrameworkCore;

namespace Meal.Data
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Meal> Meals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Models.Meal>().ToTable(nameof(Meal)).HasKey(prop => prop.Id);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(nameof(User));
                entity.HasMany(user => user.Orders).WithOne(order => order.User);
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable(nameof(Order));
                entity.HasKey(item => item.Id);
                entity.HasMany(order => order.OrderItems).WithOne();
            });
            modelBuilder.Entity<OrderItem>().ToTable(nameof(OrderItem)).HasKey(item => item.Id);
        }
    }
}