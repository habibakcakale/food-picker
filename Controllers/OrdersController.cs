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
        public async Task<IActionResult> Post([FromBody] OrderViewModel model)
        {
            model.Date = model.Date.Date;
            var userId = User.GetId();
            var order = await dbContext.Orders.FirstOrDefaultAsync(item =>
                item.UserId == userId && item.Date == model.Date);
            if (order != null)
            {
                dbContext.Orders.Remove(order);
            }

            if (!await dbContext.Users.AnyAsync(item => item.UserId == userId))
            {
                order = MapOrder(model, userId);
                order.User = new User {UserId = userId, FullName = User.Identity?.Name};
            }
            else
            {
                order = MapOrder(model, userId);
            }

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            return Ok(MapOrder(order));
        }

        private static OrderViewModel MapOrder(Order order) => new()
        {
            Date = order.Date ?? DateTime.Today,
            Id = order.Id,
            OrderItems = order.OrderItems.ToArray()
        };

        private static Order MapOrder(OrderViewModel model, string userId)
        {
            return new()
            {
                UserId = userId,
                Date = model.Date,
                OrderItems = model.OrderItems.Where(item => item != null).Select(item =>
                {
                    item.Id = 0;
                    return item;
                }).ToList()
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? dateTime)
        {
            var date = dateTime?.Date ?? DateTime.UtcNow.AddHours(3).Date;
            var items = await dbContext.Orders.Include(item => item.OrderItems).Where(item => item.Date == date)
                .ToListAsync();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await dbContext.Orders.FindAsync(id);
            if (item.UserId != User.GetId())
                return BadRequest();

            dbContext.Remove(item);
            await dbContext.SaveChangesAsync();
            return Ok(item);
        }
    }
}