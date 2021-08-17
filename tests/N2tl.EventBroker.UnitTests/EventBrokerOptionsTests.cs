using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.EventBroker.UnitTests
{
    public class EventBrokerOptionsTests
    {
        [Fact]
        public void AddEventInterrupterShouldAddInterrupters()
        {
            var options = new EventBrokerOptions();
            options.AddEventInterrupter<object>(t => Task.FromResult(true));
            options.Interrupters.Should().HaveCount(1);
        }

        [Fact]
        public void AddGeneralInterrupterShouldAddInterrupters()
        {
            var options = new EventBrokerOptions();
            options.AddGeneralInterrupter(t => Task.FromResult(true));
            options.Interrupters.Should().HaveCount(1);
        }
    }
}
