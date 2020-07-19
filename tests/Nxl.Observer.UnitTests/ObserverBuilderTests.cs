using FluentAssertions;
using Xunit;

namespace Nxl.Observer.UnitTests
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
