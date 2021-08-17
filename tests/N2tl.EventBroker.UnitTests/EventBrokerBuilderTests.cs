using System;
using FluentAssertions;
using Xunit;

namespace N2tl.EventBroker.UnitTests
{
    public class EventBrokerBuilderTests
    {
        [Fact]
        public void BuildShouldCreateAnInstanceOfIEventBroker()
        {
            var eventBroker = EventBrokerBuilder.Build();
            eventBroker.Should().NotBeNull();
        }
    }
}
