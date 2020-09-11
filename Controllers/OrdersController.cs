using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly FoodDbContext dbContext;

        public OrdersController(FoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Post(OrderItem[] orderItems)
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
            var date = dateTime ?? GetTurkeyDate();
            var items = await dbContext.Orders.Include(item => item.OrderItems).Where(item => item.Date == date).ToListAsync();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = dbContext.Orders.Attach(new Order() {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }

        private static DateTime GetTurkeyDate()
        {
            return DateTime.UtcNow.AddHours(3).Date;
        }
    }

    public static class IdentityExtensions
    {
        public static string GetId(this ClaimsPrincipal identity) => identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}