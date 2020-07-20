using FoodApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Data
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<TodaySelection> Selections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FoodItem>().Property(item => item.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<TodaySelection>().Property(item => item.Id).ValueGeneratedOnAdd();
        }
    }
}