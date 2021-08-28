using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.EventBroker.UnitTests
{
    public class EventBrokerTests
    {
        [Fact]
        public void ConstructorShouldNotThrowWithoutParams()
        {
            this.Invoking(_ => new EventBroker())
                .Should().NotThrow();
        }

        [Fact]
        public void ConstructorShouldNotThrowWithParams()
        {
            this.Invoking(_ => new EventBroker(new object(), new object()))
                .Should().NotThrow();
        }

        [Fact]
        public async Task SubscribeEventShouldReceiveEvents()
        {
            var expectedResult = new MockEvent { Name = "test" };
            MockEvent actualResult = null;

            var callback = new Func<MockEvent, Task>(obj =>
            {
                actualResult = obj;
                return Task.CompletedTask;
            });

            var eventBroker = new EventBroker();
            eventBroker.SubscribeEvent(callback);

            await eventBroker.SendEvent(expectedResult);
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async Task SubscribeResultShouldReceiveResults()
        {
            var expectedEvent = new MockEvent { Name = "test" };
            var expectedResult = new MockResult { Email = "a@b.c" };
            MockEvent actualEvent = null;
            MockResult actualResult = null;

            var callback = new Func<MockEvent, MockResult, Task>((evt, rst) =>
            {
                actualEvent = evt;
                actualResult = rst;
                return Task.CompletedTask;
            });

            var eventBroker = new EventBroker();
            eventBroker.SubscribeResult(callback);

            await eventBroker.SendResult(expectedEvent, expectedResult);
            actualEvent.Should().Be(expectedEvent);
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async Task UnsubscribeEventShouldStopReceiving()
        {
            var numberOfCalls = 0;
            var callback = new Func<MockEvent, Task>(obj =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            var eventBroker = new EventBroker();
            eventBroker.SubscribeEvent(callback);
            await eventBroker.SendEvent(new MockEvent { Name = "test" });
            numberOfCalls.Should().Be(1);

            eventBroker.UnsubscribeEvent(callback);
            await eventBroker.SendEvent(new MockEvent { Name = "test" });
            numberOfCalls.Should().Be(1);
        }

        [Fact]
        public async Task UnsubscribeResultShouldStopReceiving()
        {
            var numberOfCalls = 0;
            var callback = new Func<MockEvent, MockResult, Task>((evt, rst) =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            var eventBroker = new EventBroker();
            eventBroker.SubscribeResult(callback);
            await eventBroker.SendResult(
                new MockEvent { Name = "test" }, new MockResult { Email = "a@b.c" });
            numberOfCalls.Should().Be(1);

            eventBroker.UnsubscribeResult(callback);
            await eventBroker.SendResult(
                new MockEvent { Name = "test" }, new MockResult { Email = "a@b.c" });
            numberOfCalls.Should().Be(1);
        }

        public class MockEvent
        {
            public string Name { get; set; }
        }

        public class MockResult
        {
            public string Email { get; set; }
        }
    }
}
