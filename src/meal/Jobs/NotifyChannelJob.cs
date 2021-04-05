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
    using User = Models.User;

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
                select new User {Id = user.Id, SlackId = user.SlackId};
            var users = await query.ToListAsync();
            if (users.Any()) {
                var topPicks = await GetTopPicks();
                await SelectMealForUsers(users, topPicks);
                await NotifyChannel(users, topPicks);
            }
        }

        private async Task SelectMealForUsers(IEnumerable<User> users, IReadOnlyCollection<OrderItem> topPicks) {
            var orders = users.Select(user => new Order {
                UserId = user.Id,
                Date = DateTime.UtcNow.Date,
                OrderItems = topPicks.Select(item => new OrderItem {
                    Name = item.Name,
                    MealType = item.MealType
                }).ToList()
            });
            await dbContext.Orders.AddRangeAsync(orders);
            await dbContext.SaveChangesAsync();
        }

        private async Task<List<OrderItem>> GetTopPicks() {
            var topPicks = await dbContext.Orders
                .Where(item => item.Date == DateTime.Today)
                .SelectMany(item => item.OrderItems)
                .GroupBy(item => new {item.MealType, item.Name})
                .Select(item => new {item.Key.MealType, item.Key.Name, Count = item.Count()})
                .OrderBy(item => item.MealType).ThenByDescending(item => item.Count)
                .ToListAsync();
            return topPicks
                .GroupBy(item => item.MealType)
                .SelectMany(item => item.Take(item.Key == MealType.Salad ? 2 : 1))
                .Select(item => new OrderItem {Name = item.Name, MealType = item.MealType})
                .ToList();
        }

        private Task NotifyChannel(ICollection<User> users, IEnumerable<OrderItem> topPicks) {
            if (!users.Any()) {
                return Task.CompletedTask;
            }

            var taskCompletionSource = new TaskCompletionSource();

            var builder = new StringBuilder();
            foreach (var user in users) {
                builder.AppendFormat("<@{0}>{1}", user.SlackId, users.Last() == user ? string.Empty : ", ");
            }

            slackClient.PostMessage(response => {
                    if (response.ok) {
                        taskCompletionSource.TrySetResult();
                    } else {
                        logger.LogError("Error occured while sending slack message {SlackError}", response.error);
                        taskCompletionSource.TrySetException(new Exception(response.error));
                    }
                },
                slackOptions.Channel,
                string.Empty,
                attachments: new[] {
                    new Attachment {
                        color = "#c2185b",
                        pretext = $"You still have time to pick. We already picked {string.Join(", ", topPicks.Select(item => item.Name))} for you. But you can change it. :tada:",
                        text = builder.ToString()
                    }
                },
                botName: "Ruby Chef!");

            return taskCompletionSource.Task;
        }
    }
}
