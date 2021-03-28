namespace Meal.Jobs {
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models;
    using Quartz;
    using SlackAPI;

    public class NotifyChannelJob : IJob {
        private readonly FoodDbContext dbContext;

        private readonly SlackOptions slackOptions;

        public NotifyChannelJob(FoodDbContext dbContext, IOptions<SlackOptions> options) {
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
                var builder = new StringBuilder();
                foreach (var user in users) {
                    builder.AppendFormat("<@{0}>{1}", user.SlackId, users.Last() == user ? string.Empty : ", ");
                }

                slackClient.PostMessage(response => response.AssertOk(), slackOptions.Channel, string.Empty,
                    attachments: new[] {new Attachment {color = "#c2185b", pretext = "Still have time to pick.", text = builder.ToString()}},
                    botName: "Pick Food!");
            }
        }
    }
}
