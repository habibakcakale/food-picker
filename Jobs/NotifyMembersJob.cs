using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meal.Data;
using Meal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using SlackAPI;

namespace Meal.Jobs {
    public class NotifyMembersJob : IJob {
        private readonly FoodDbContext dbContext;
        private readonly SlackOptions slackOptions;

        public NotifyMembersJob(FoodDbContext dbContext, IOptions<SlackOptions> options) {
            this.dbContext = dbContext;
            this.slackOptions = options.Value;
        }

        public async Task Execute(IJobExecutionContext context) {
            var query = from user in dbContext.Users
                join order in dbContext.Orders.Where(item => item.Date == DateTime.Today) on user.Id equals order.UserId into orders
                from todayOrder in orders.DefaultIfEmpty()
                select user;
            var users = await query.ToListAsync();
            if (users.Any()) {
                var slackClient = new SlackClient(slackOptions.Token);
                var builder = new StringBuilder();
                foreach (var user in users) {
                    builder.AppendFormat("<@{0}>{1}", user.SlackId, users.Last() == user ? string.Empty : ", ");
                }

                foreach (var user in users) {
                    slackClient.PostMessage(response => response.AssertOk(), user.SlackId, string.Empty,
                        blocks: new IBlock[] {
                            new SectionBlock() {
                                text = new Text() {type = "mrkdwn", text = "Still have time to pick food."},
                                accessory = new ButtonElement() {
                                    url = "https://meal.rubygamestudio.com/today",
                                    action_id = "click",
                                    text = new Text() {text = "Pick"}
                                }
                            }
                        }, botName: "Pick Meal!");
                }
            }
        }
    }
}
