namespace Meal.Jobs {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
    using Quartz;
    using SlackAPI;

    public class NotifyChannelJob : IJob {
        private readonly FoodDbContext dbContext;
        private readonly SlackClient slackClient;
        private readonly ILogger<NotifyChannelJob> logger;

        private readonly SlackOptions slackOptions;

        public NotifyChannelJob(FoodDbContext dbContext, SlackClient slackClient, IOptions<SlackOptions> options, ILogger<NotifyChannelJob> logger) {
            this.dbContext = dbContext;
            this.slackClient = slackClient;
            this.logger = logger;
            this.slackOptions = options.Value;
        }

        public async Task Execute(IJobExecutionContext context) {
            var query = from user in dbContext.Users
                join order in dbContext.Orders.Where(item => item.Date == DateTime.Today) on user.Id equals order.UserId into orders
                from todayOrder in orders.DefaultIfEmpty()
                where !string.IsNullOrWhiteSpace(user.SlackId) && todayOrder == null
                select user.SlackId;
            var users = await query.ToListAsync();
            await NotifyChannel(users);
        }

        private Task NotifyChannel(IReadOnlyCollection<string> users) {
            var taskCompletionSource = new TaskCompletionSource();
            if (!users.Any()) {
                return taskCompletionSource.Task;
            }

            var builder = new StringBuilder();
            foreach (var slackId in users) {
                builder.AppendFormat("<@{0}>{1}", slackId, users.Last() == slackId ? string.Empty : ", ");
            }

            slackClient.PostMessage(response => {
                    if (response.ok) {
                        taskCompletionSource.TrySetResult();
                    } else {
                        logger.LogError("Error occured while sending slack message {SlackError}", response.error);
                        taskCompletionSource.TrySetException(new Exception(response.error));
                    }
                }, slackOptions.Channel, string.Empty,
                attachments: new[] {new Attachment {color = "#c2185b", pretext = "Still have time to pick.", text = builder.ToString()}},
                botName: "Pick Food!");

            return taskCompletionSource.Task;
        }
    }
}
