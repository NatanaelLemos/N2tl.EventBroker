using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class SubscribeUnsubscribeTests
    {
        private List<string> _result = new List<string>();

        [Fact]
        public async Task NotifyShouldStopAfterUnsubscription()
        {
            var eventBroker = new EventBroker();
            eventBroker.Subscribe<SubscribeUnsubscribeTestsEvent>(Test1);
            eventBroker.Subscribe<SubscribeUnsubscribeTestsEvent>(Test2);
            eventBroker.Subscribe<SubscribeUnsubscribeTestsEvent>(Test3);

            await eventBroker.Notify(new SubscribeUnsubscribeTestsEvent());
            _result.Should().BeEquivalentTo(new List<string>
            {
                "1", "2", "3"
            });

            eventBroker.Unsubscribe<SubscribeUnsubscribeTestsEvent>(Test2);

            await eventBroker.Notify(new SubscribeUnsubscribeTestsEvent());
            _result.Should().BeEquivalentTo(new List<string>
            {
                "1", "2", "3", "1", "3"
            });
        }

        private Task Test1(SubscribeUnsubscribeTestsEvent evt)
        {
            _result.Add("1");
            return Task.CompletedTask;
        }

        private Task Test2(SubscribeUnsubscribeTestsEvent evt)
        {
            _result.Add("2");
            return Task.CompletedTask;
        }

        private Task Test3(SubscribeUnsubscribeTestsEvent evt)
        {
            _result.Add("3");
            return Task.CompletedTask;
        }
    }

    public class SubscribeUnsubscribeTestsEvent
    {
        public string Value { get; set; }
    }
}
