using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class SubscribeUnsubscribeQueriesTests
    {
        private List<string> _result = new List<string>();

        [Fact]
        public async Task QueryShouldStopAfterUnsubscription()
        {
            var eventBroker = new EventBroker();
            eventBroker.Subscribe<SubscribeUnsubscribeTestsQuery, object>(Test1);
            eventBroker.Subscribe<SubscribeUnsubscribeTestsQuery, object>(Test2);
            eventBroker.Subscribe<SubscribeUnsubscribeTestsQuery, object>(Test3);

            var test = await eventBroker.Query<SubscribeUnsubscribeTestsQuery, object>(new SubscribeUnsubscribeTestsQuery());
            _result.Should().BeEquivalentTo(new List<string>
            {
                "1", "2", "3"
            });

            eventBroker.Unsubscribe<SubscribeUnsubscribeTestsQuery, object>(Test2);

            test = await eventBroker.Query<SubscribeUnsubscribeTestsQuery, object>(new SubscribeUnsubscribeTestsQuery());
            _result.Should().BeEquivalentTo(new List<string>
            {
                "1", "2", "3", "1", "3"
            });
        }

        private Task<object> Test1(SubscribeUnsubscribeTestsQuery evt)
        {
            _result.Add("1");
            return Task.FromResult(new object());
        }

        private Task<object> Test2(SubscribeUnsubscribeTestsQuery evt)
        {
            _result.Add("2");
            return Task.FromResult(new object());
        }

        private Task<object> Test3(SubscribeUnsubscribeTestsQuery evt)
        {
            _result.Add("3");
            return Task.FromResult(new object());
        }
    }

    public class SubscribeUnsubscribeTestsQuery
    {
        public string Value { get; set; }
    }
}
