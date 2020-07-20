using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class ObserverBuilderTests
    {
        [Fact]
        public void BuildShouldCreateAnInstanceOfIEventBroker()
        {
            var eventBroker = ObserverBuilder.Build();
            eventBroker.Should().NotBeNull();
        }
    }
}
