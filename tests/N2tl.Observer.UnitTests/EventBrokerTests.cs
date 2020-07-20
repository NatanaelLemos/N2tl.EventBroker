using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class EventBrokerTests
    {
        [Theory]
        [InlineData("nothing")]
        [InlineData("null")]
        [InlineData("wrongfunc")]
        [InlineData("func")]
        public void ConstructorShouldNotThrow(string mockType)
        {
            switch (mockType)
            {
                case "nothing":
                    this.Invoking(_ => new EventBroker())
                        .Should().NotThrow();
                    break;
                case "null":
                    this.Invoking(_ => new EventBroker(null))
                        .Should().NotThrow();
                    break;
                case "wrongfunc":
                    object wrongFuncType = new object();
                    this.Invoking(_ => new EventBroker(wrongFuncType))
                        .Should().NotThrow();
                    break;
                case "func":
                    Func<EventBrokerTests, Task<bool>> function = t => Task.FromResult(true);
                    this.Invoking(_ => new EventBroker(function))
                        .Should().NotThrow();
                    break;
                default:
                    throw new Exception("Invalid test type");
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
            Func<object, Task<bool>> func = u => Task.FromResult(false);
            using var eventBroker = new EventBroker(func);

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
            Func<object, Task<bool>> func = u => Task.FromResult(true);
            using var eventBroker = new EventBroker(func);

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
