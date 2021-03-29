namespace Meal.Tests {
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using SlackAPI;
    using Xunit;

    public class NotifyMembersJobTasks : IClassFixture<WebApplicationFactory<Startup>> {
        private readonly WebApplicationFactory<Startup> factory;

        public NotifyMembersJobTasks(WebApplicationFactory<Startup> factory) => this.factory = factory;

        [Fact]
        public void SlackClientShouldBeResolved() {
            var scope = factory.Services.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<SlackClient>();
            Assert.NotNull(job);
        }
    }
}
