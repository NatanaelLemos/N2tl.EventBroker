using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace System.Nxl.Observer.UnitTests
{
    public class ObserverExtensionsTests
    {
        [Fact]
        public void AddObserverShouldAddAnInstanceOfIEventBroker()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddObserver();
            serviceCollection.BuildServiceProvider()
                .GetService<IEventBroker>().Should().NotBeNull();
        }
    }
}
