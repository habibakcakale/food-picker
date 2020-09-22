using System;
using System.Linq;
using System.Threading.Tasks;
using Meal.Data;
using Meal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meal.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly FoodDbContext dbContext;

        public OrdersController(FoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderItem[] orderItems)
        {
            var date = GetTurkeyDate();
            var userId = User.GetId();
            var order = await dbContext.Orders.FirstOrDefaultAsync(item => item.UserId == userId && item.Date == date);
            if (order != null)
            {
                dbContext.Orders.Remove(order);
            }

            order = new Order
            {
                FullName = User.Identity.Name,
                UserId = userId,
                Date = date,
                OrderItems = orderItems.Select(item =>
                {
                    item.Id = 0;
                    return item;
                }).ToList()
            };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? dateTime)
        {
            var date = dateTime?.Date ?? GetTurkeyDate();
            var items = await dbContext.Orders.Include(item => item.OrderItems).Where(item => item.Date == date).ToListAsync();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = dbContext.Orders.Attach(new Order {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }

        private static DateTime GetTurkeyDate()
        {
            return DateTime.UtcNow.AddHours(3).Date;
        }
    }
}