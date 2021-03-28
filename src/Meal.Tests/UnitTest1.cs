namespace Meal.Tests {
    using Jobs;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class NotifyMembersJobTasks : IClassFixture<WebApplicationFactory<Startup>> {
        private readonly WebApplicationFactory<Startup> factory;

        public NotifyMembersJobTasks(WebApplicationFactory<Startup> factory) => this.factory = factory;

        [Fact]
        public async void Test1() {
            var scope = factory.Services.CreateScope();
            var job = (NotifyMembersJob)scope.ServiceProvider.GetRequiredService<NotifyMembersJob>();
            await job.Execute(null);
        }
    }
}
