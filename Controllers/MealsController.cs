using System.Linq;
using System.Threading.Tasks;
using Meal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meal.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase {
        private readonly FoodDbContext dbContext;

        public MealsController(FoodDbContext dbContext) {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get() {
            var items = dbContext.Meals.ToList();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var item = dbContext.Meals.Attach(new Models.Meal() {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Meal[] foodItems) {
            dbContext.Meals.AddRange(foodItems);
            await dbContext.SaveChangesAsync();
            return Ok(foodItems);
        }
    }
}
