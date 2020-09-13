using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class CommandBrokerTests
    {
        [Theory]
        [InlineData("nothing")]
        [InlineData("null")]
        [InlineData("wrongfunc")]
        [InlineData("func")]
        public void ConstructorShouldNotThrow(string mockType)
        {
            switch (mockType)
            {
                case "nothing":
                    this.Invoking(_ => new EventBroker())
                        .Should().NotThrow();
                    break;
                case "null":
                    this.Invoking(_ => new EventBroker(null))
                        .Should().NotThrow();
                    break;
                case "wrongfunc":
                    object wrongFuncType = new object();
                    this.Invoking(_ => new EventBroker(wrongFuncType))
                        .Should().NotThrow();
                    break;
                case "func":
                    Func<CommandBrokerTests, Task<bool>> function = t => Task.FromResult(true);
                    this.Invoking(_ => new EventBroker(function))
                        .Should().NotThrow();
                    break;
                default:
                    throw new Exception("Invalid test type");
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SubscribeShouldNotThrow(bool shouldMockCallback)
        {
            using var eventBroker = new EventBroker(null);

            if (shouldMockCallback)
            {
                eventBroker.Invoking(b => b.Subscribe<CommandBrokerTests>(c => Task.CompletedTask))
                    .Should().NotThrow();
            }
            else
            {
                eventBroker.Invoking(b => b.Subscribe<CommandBrokerTests>(null))
                    .Should().NotThrow();
            }
        }

        [Fact]
        public void CommandShouldNotThrowIfTheresNoSubscription()
        {
            using var eventBroker = new EventBroker(null);
            eventBroker.Awaiting(e => e.Command(new CommandBrokerTests()))
                .Should().NotThrow();
        }

        [Fact]
        public async Task CommandShouldNotifySubscribersIfTheresNoAuthenticationFactory()
        {
            using var eventBroker = new EventBroker(null);

            var wasCalled = false;
            eventBroker.Subscribe<CommandBrokerTests>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Command(new CommandBrokerTests());

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task CommandShouldNotNotifySubscribersIfEventInterrupterBlocks()
        {
            Func<CommandBrokerTests, Task<bool>> func = u => Task.FromResult(false);
            using var eventBroker = new EventBroker(func);

            var wasCalled = false;
            eventBroker.Subscribe<CommandBrokerTests>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Command(new CommandBrokerTests());
            wasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task CommandShouldNotifySubscribersIfEventInterrupterDoesNotBlock()
        {
            Func<CommandBrokerTests, Task<bool>> func = u => Task.FromResult(true);
            using var eventBroker = new EventBroker(func);

            var wasCalled = false;
            eventBroker.Subscribe<CommandBrokerTests>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Command(new CommandBrokerTests());
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task CommandShouldNotNotifySubscribersIfGeneralInterrupterBlocks()
        {
            Func<object, Task<bool>> func = u => Task.FromResult(false);
            using var eventBroker = new EventBroker(func);

            var wasCalled = false;
            eventBroker.Subscribe<CommandBrokerTests>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Command(new CommandBrokerTests());
            wasCalled.Should().BeFalse();
        }

        [Fact]
        public async Task CommandShouldNotifySubscribersIfGeneralInterrupterDoesNotBlock()
        {
            Func<object, Task<bool>> func = u => Task.FromResult(true);
            using var eventBroker = new EventBroker(func);

            var wasCalled = false;
            eventBroker.Subscribe<CommandBrokerTests>(o =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await eventBroker.Command(new CommandBrokerTests());
            wasCalled.Should().BeTrue();
        }
    }
}
