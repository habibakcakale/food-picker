namespace Meal {
    using System;
    using Microsoft.Extensions.DependencyInjection;
    
    using Quartz;
    using Jobs;

    public static class StartupExtensions {
        public static void ConfigureQuartz(this IServiceCollection services) {
            services.AddQuartz(configurator => {
                configurator.UseMicrosoftDependencyInjectionJobFactory(options => {
                    options.CreateScope = true;
                    options.AllowDefaultConstructor = true;
                });
                configurator.AddJob<NotifyChannelJob>(JobKey.Create(nameof(NotifyChannelJob)));
                configurator.AddJob<NotifyMembersJob>(JobKey.Create(nameof(NotifyMembersJob)));
                configurator.AddTrigger(triggerConfigurator =>
                        triggerConfigurator
                            .ForJob(nameof(NotifyChannelJob))
                            .WithCronSchedule("0 0 14 ? * 2-6 *", cr => cr.InTimeZone(TimeZoneInfo.Utc)) // every week day at 5PM in TR
                );
                configurator.AddTrigger(triggerConfigurator =>
                        triggerConfigurator
                            .ForJob(nameof(NotifyMembersJob))
                            .WithCronSchedule("0 30 13 ? * 2-6 *", cr => cr.InTimeZone(TimeZoneInfo.Utc)) // every week day at 4.30PM in TR
                );
            });
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }
    }
}
