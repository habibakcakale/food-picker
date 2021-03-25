using Meal.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Meal {
    public static class StartupExtensions {
        public static void ConfigureQuartz(this IServiceCollection services) {
            services.AddQuartz(configurator => {
                configurator.UseMicrosoftDependencyInjectionJobFactory(options => {
                    options.CreateScope = true;
                    options.AllowDefaultConstructor = true;
                });
                configurator.AddJob<NotifyChannelJob>(JobKey.Create(nameof(NotifyChannelJob)));
                configurator.AddJob<NotifyMembersJob>(JobKey.Create(nameof(NotifyMembersJob)));
                configurator.AddTrigger(triggerConfigurator => triggerConfigurator.ForJob(nameof(NotifyChannelJob)).WithCronSchedule("0 0 17 ? * 2-6 *")); // every week day at 5PM
                configurator.AddTrigger(triggerConfigurator => triggerConfigurator.ForJob(nameof(NotifyMembersJob)).WithCronSchedule("0 30 16 ? * 2-6 *")); // every week day at 4.30PM
            });
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }
    }
}
