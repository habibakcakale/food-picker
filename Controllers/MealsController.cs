using System.Linq;
using System.Threading.Tasks;
using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FoodApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly FoodDbContext dbContext;

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
                {NamingStrategy = new CamelCaseNamingStrategy()}
        };

        public MealsController(FoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await dbContext.Meals.ToListAsync();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = dbContext.Meals.Attach(new Meal() {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Meal[] foodItems)
        {
            dbContext.Meals.AddRange(foodItems);
            await dbContext.SaveChangesAsync();
            return Ok(foodItems);
        }
    }
}