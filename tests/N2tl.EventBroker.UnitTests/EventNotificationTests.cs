using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.EventBroker.UnitTests
{
    public class EventNotificationTests
    {
        [Fact]
        public async Task SubscribeShouldAddListener()
        {
            var hasCalled = false;
            var notification = new EventNotification<object>();
            notification.Subscribe(_ =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            await notification.Notify(new object());
            hasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task UnsubscribeShouldRemoveListener()
        {
            var hasCalled = false;

            var notification = new EventNotification<object>();
            var callback = new Func<object, Task>(_ =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            notification.Subscribe(callback);
            await notification.Notify(new object());
            hasCalled.Should().BeTrue();

            hasCalled = false;
            notification.Unsubscribe(callback);
            await notification.Notify(new object());
            hasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task DisposeShouldRemoveAllListeners()
        {
            var hasCalled = false;

            var notification = new EventNotification<object>();
            var callback = new Func<object, Task>(_ =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            notification.Subscribe(callback);
            await notification.Notify(new object());
            hasCalled.Should().BeTrue();

            hasCalled = false;
            notification.Dispose();
            await notification.Notify(new object());
            hasCalled.Should().BeFalse();
        }
    }
}
