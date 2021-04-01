namespace Meal.Jobs {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Quartz;
    using SlackAPI;
    using Data;
    using Microsoft.Extensions.Logging;
    using Models;

    public class NotifyMembersJob : IJob {
        private readonly FoodDbContext dbContext;
        private readonly SlackClient slackClient;
        private readonly ILogger<NotifyMembersJob> logger;
        private readonly SlackOptions slackOptions;

        public NotifyMembersJob(FoodDbContext dbContext, SlackClient slackClient, IOptions<SlackOptions> options, ILogger<NotifyMembersJob> logger) {
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
                select user;
            var users = await query.ToListAsync();
            if (users.Any()) {
                foreach (var user in users) {
                    slackClient.PostMessage(response => {
                            if (!response.ok) {
                                logger.LogError("Error occured while sending slack message {SlackError}", response.error);
                            }
                        }, user.SlackId, string.Empty,
                        blocks: new IBlock[] {
                            new SectionBlock {
                                text = new Text {
                                    type = "mrkdwn",
                                    text = "Still have time to pick a meal."
                                },
                                accessory = new ButtonElement {
                                    text = new Text {type = "plain_text", text = "Pick", emoji = true},
                                    value = "pick_meal",
                                    url = slackOptions.Url,
                                    action_id = "button-pick-meal"
                                }
                            }
                        });
                }
            }
        }
    }
}
