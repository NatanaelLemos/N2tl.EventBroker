using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class ObserverOptionsTests
    {
        [Fact]
        public void AddInterrupterShouldAddInterrupters()
        {
            var options = new ObserverOptions();
            options.AddInterrupter<ObserverBuilderTests>(t => Task.FromResult(true));
            options.Interrupters.Should().HaveCount(1);
        }
    }
}
