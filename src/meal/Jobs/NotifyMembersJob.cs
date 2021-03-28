namespace Meal.Jobs {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Quartz;
    using SlackAPI;
    using Data;
    using Models;

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
                where !string.IsNullOrWhiteSpace(user.SlackId)
                select user;
            var users = await query.ToListAsync();
            if (users.Any()) {
                var slackClient = new SlackClient(slackOptions.Token);
                foreach (var user in users) {
                    slackClient.PostMessage(response => response.AssertOk(), user.SlackId, string.Empty,
                        blocks: new IBlock[] {
                            new SectionBlock() {
                                text = new Text {type = "mrkdwn", text = "Still have time to pick a meal."},
                                accessory = new ButtonElement {
                                    url = slackOptions.Url,
                                    action_id = "click",
                                    text = new Text() {text = "Pick"}
                                }
                            }
                        });
                }
            }
        }
    }
}
