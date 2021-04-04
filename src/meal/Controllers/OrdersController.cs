namespace Meal.Controllers {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Data;
    using Models;

    [Authorize]
    [Route("[controller]")]
    public class OrdersController : ControllerBase {
        private readonly FoodDbContext dbContext;
        public OrdersController(FoodDbContext dbContext) => this.dbContext = dbContext;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderViewModel model) {
            model.Date = model.Date.Date;
            var userId = User.GetId();
            await CheckUser(userId);
            var order = await dbContext.Orders.FirstOrDefaultAsync(item => item.UserId == userId && item.Date == model.Date);
            if (order != null) {
                dbContext.Orders.Remove(order);
            }

            order = MapOrder(model, userId);

            var entityRef = dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            await entityRef.Reference(item => item.User).LoadAsync();
            return Ok(MapOrder(entityRef.Entity));
        }

        /// <summary>
        /// Migration purpose only
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task CheckUser(string userId) {
            if (!await dbContext.Users.AnyAsync(item => item.Id == userId)) {
                var user = User.ToUser();
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }

        private static OrderViewModel MapOrder(Order order) {
            var vm = new OrderViewModel {
                Id = order.Id,
                UserId = order.UserId,
                Date = order.Date ?? DateTime.Today,
                OrderItems = order.OrderItems.ToArray()
            };
            if (order.User != null) {
                vm.User = new User {
                    Id = order.User.Id,
                    SlackId = order.User.SlackId,
                    Email = order.User.Email,
                    Name = order.User.Name,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Picture = order.User.Picture
                };
            }

            return vm;
        }

        private static Order MapOrder(OrderViewModel model, string userId) {
            return new() {
                UserId = userId,
                Date = model.Date,
                OrderItems = model.OrderItems.Where(item => item != null).Select(item => {
                    item.Id = 0;
                    return item;
                }).ToList()
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? dateTime) {
            var date = dateTime?.Date ?? DateTime.UtcNow.AddHours(3).Date;
            var items = await dbContext.Orders.Include(item => item.OrderItems)
                .Include(item => item.User)
                .Where(item => item.Date == date)
                .ToListAsync();
            return Ok(items.Select(MapOrder));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var item = await dbContext.Orders.FindAsync(id);
            if (item.UserId != User.GetId())
                return BadRequest();

            dbContext.Remove(item);
            await dbContext.SaveChangesAsync();
            return Ok(item);
        }
    }
}
