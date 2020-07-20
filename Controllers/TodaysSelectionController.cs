using System;
using System.Linq;
using System.Threading.Tasks;
using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodaysSelectionController : ControllerBase
    {
        private readonly FoodDbContext dbContext;

        public TodaysSelectionController(FoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Post(TodaySelection selection)
        {
            selection.Date = DateTime.UtcNow.AddHours(+3).Date;
            dbContext.Selections.Add(selection);
            await dbContext.SaveChangesAsync();
            return Ok(selection);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var date = DateTime.UtcNow.AddHours(+3).Date;
            var items = await dbContext.Selections.Where(item => item.Date == date).ToListAsync();
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = dbContext.Selections.Attach(new TodaySelection() {Id = id});
            item.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
            return Ok(item.Entity);
        }
    }
}