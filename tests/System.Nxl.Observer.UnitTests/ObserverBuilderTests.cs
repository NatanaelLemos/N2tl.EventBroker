using FluentAssertions;
using Xunit;

namespace System.Nxl.Observer.UnitTests
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
