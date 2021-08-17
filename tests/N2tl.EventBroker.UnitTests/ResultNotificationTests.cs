using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.EventBroker.UnitTests
{
    public class ResultNotificationTests
    {
        [Fact]
        public async Task SubscribeShouldAddListener()
        {
            var hasCalled = false;
            var notification = new ResultNotification<object, object>();
            notification.Subscribe((_1, _2) =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            await notification.Notify(new object(), new object());
            hasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task UnsubscribeShouldRemoveListener()
        {
            var hasCalled = false;

            var notification = new ResultNotification<object, object>();
            var callback = new Func<object, object, Task>((_1, _2) =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            notification.Subscribe(callback);
            await notification.Notify(new object(), new object());
            hasCalled.Should().BeTrue();

            hasCalled = false;
            notification.Unsubscribe(callback);
            await notification.Notify(new object(), new object());
            hasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task DisposeShouldRemoveAllListeners()
        {
            var hasCalled = false;

            var notification = new ResultNotification<object, object>();
            var callback = new Func<object, object, Task>((_1, _2) =>
            {
                hasCalled = true;
                return Task.CompletedTask;
            });

            notification.Subscribe(callback);
            await notification.Notify(new object(), new object());
            hasCalled.Should().BeTrue();

            hasCalled = false;
            notification.Dispose();
            await notification.Notify(new object(), new object());
            hasCalled.Should().BeFalse();
        }
    }
}
