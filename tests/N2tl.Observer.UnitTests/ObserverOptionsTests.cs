using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class ObserverOptionsTests
    {
        [Fact]
        public void AddEventInterrupterShouldAddInterrupters()
        {
            var options = new ObserverOptions();
            options.AddEventInterrupter<ObserverBuilderTests>(t => Task.FromResult(true));
            options.Interrupters.Should().HaveCount(1);
        }

        [Fact]
        public void AddGeneralInterrupterShouldAddInterrupters()
        {
            var options = new ObserverOptions();
            options.AddGeneralInterrupter(t => Task.FromResult(true));
            options.Interrupters.Should().HaveCount(1);
        }
    }
}
