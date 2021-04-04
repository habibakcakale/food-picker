namespace Meal.Controllers {
    using System.Linq;
    using System.Threading.Tasks;
    using Meal.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase {
        private readonly FoodDbContext dbContext;
        private readonly IOptions<SlackOptions> slackOptions;

        public MealsController(FoodDbContext dbContext, IOptions<SlackOptions> slackOptions) {
            this.dbContext = dbContext;
            this.slackOptions = slackOptions;
        }

        [HttpGet]
        public IActionResult Get() {
            var items = dbContext.Meals.ToList();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var item = dbContext.Meals.Attach(new Meal {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Meal[] foodItems) {
            dbContext.Meals.AddRange(foodItems);
            await dbContext.SaveChangesAsync();
            return Ok(foodItems);
        }

        [HttpGet("slack-options")]
        public IActionResult OnGetSlackOptions() => Ok(slackOptions.Value);
    }
}
