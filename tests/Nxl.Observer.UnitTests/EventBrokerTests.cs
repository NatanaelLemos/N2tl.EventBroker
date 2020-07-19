using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Nxl.Observer.UnitTests
{
    public class EventBrokerTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ConstructorShouldNotThrow(bool shouldMockAuthFactory)
        {
            if (shouldMockAuthFactory)
            {
                this.Invoking(_ => new EventBroker(t => Task.FromResult(true)))
                    .Should().NotThrow();
            }
            else
            {
                this.Invoking(_ => new EventBroker(null))
                    .Should().NotThrow();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SubscribeShouldNotThrow(bool shouldMockCallback)
        {
            using var eventBroker = new EventBroker(null);

            if (shouldMockCallback)
            {
                eventBroker.Invoking(b => b.Subscribe<object>(c => Task.CompletedTask))
                    .Should().NotThrow();
            }
            else
            {
                eventBroker.Invoking(b => b.Subscribe<object>(null))
                    .Should().NotThrow();
            }
        }

        [Fact]
        public void NotifyShouldNotThrowIfTheresNoSubscription()
        {
            using var eventBroker = new EventBroker(null);
            eventBroker.Awaiting(e => e.Notify(new object()))
                .Should().NotThrow();
        }

        [Fact]
        public async Task NotifyShouldNotifySubscribersIfTheresNoAuthenticationFactory()
        {
            using var eventBroker = new EventBroker(null);

            var wasCalled = false;
            eventBroker.Subscribe<object>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Notify(new object());

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task NotifyShouldNotNotifySubscribersIfNotAuthenticated()
        {
            using var eventBroker = new EventBroker(u => Task.FromResult(false));

            var wasCalled = false;
            eventBroker.Subscribe<object>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Notify(new object());

            wasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task NotifyShouldNotifySubscribersIfIsAuthenticated()
        {
            using var eventBroker = new EventBroker(u => Task.FromResult(true));

            var wasCalled = false;
            eventBroker.Subscribe<object>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Notify(new object());

            wasCalled.Should().BeTrue();
        }
    }
}
