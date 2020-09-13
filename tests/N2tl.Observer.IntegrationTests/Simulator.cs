using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using N2tl.Observer.IntegrationTests.Events.Users;
using Xunit;

namespace N2tl.Observer.IntegrationTests
{
    public class Simulator
    {
        [Fact]
        public async Task Simulate()
        {
            var realEventBroker = ObserverBuilder.Build();
            var testEventBroker = new Mock<IEventBroker>();
            MockCalls(realEventBroker, testEventBroker);

            var userEventHandler = new UserEventHandler(testEventBroker.Object);
            StartUserEventHandler(userEventHandler, testEventBroker);

            var user = await CreateUser(userEventHandler, testEventBroker);
            await QueryUsers(user.Email, testEventBroker);

            StopUserEventHandler(userEventHandler, testEventBroker);
            await DoNotCreateUser(userEventHandler, testEventBroker);
            await QueryNullUsers(user.Email, testEventBroker);
        }

        private void MockCalls(IEventBroker realEventBroker, Mock<IEventBroker> testEventBroker)
        {
            testEventBroker
                .Setup(t => t.Subscribe(It.IsAny<Func<CreateUserCommand, Task>>()))
                .Callback<Func<CreateUserCommand, Task>>(t => realEventBroker.Subscribe(t));

            testEventBroker
                .Setup(t => t.Command(It.IsAny<CreateUserCommand>()))
                .Callback<CreateUserCommand>(t => realEventBroker.Command(t));

            testEventBroker
                .Setup(t => t.Unsubscribe(It.IsAny<Func<CreateUserCommand, Task>>()))
                .Callback<Func<CreateUserCommand, Task>>(t => realEventBroker.Unsubscribe(t));

            testEventBroker
                .Setup(t => t.Subscribe(It.IsAny<Func<UserQuery, Task<List<UserDto>>>>()))
                .Callback<Func<UserQuery, Task<List<UserDto>>>>(t => realEventBroker.Subscribe(t));

            testEventBroker
                .Setup(t => t.Query<UserQuery, List<UserDto>>(It.IsAny<UserQuery>()))
                .Returns<UserQuery>(t => realEventBroker.Query<UserQuery, List<UserDto>>(t));

            testEventBroker
                .Setup(t => t.Unsubscribe(It.IsAny<Func<UserQuery, Task<List<UserDto>>>>()))
                .Callback<Func<UserQuery, Task<List<UserDto>>>>(t => realEventBroker.Unsubscribe(t));
        }

        private void StartUserEventHandler(UserEventHandler userEventHandler, Mock<IEventBroker> testEventBroker)
        {
            userEventHandler.Start();
            testEventBroker.Verify(t => t.Subscribe(It.IsAny<Func<CreateUserCommand, Task>>()), Times.Once);
            testEventBroker.Verify(t => t.Subscribe(It.IsAny<Func<UserQuery, Task<List<UserDto>>>>()), Times.Once);
        }

        private async Task<UserDto> CreateUser(UserEventHandler userEventHandler, Mock<IEventBroker> testEventBroker)
        {
            var user = new UserDto
            {
                Email = "user@email.com"
            };

            userEventHandler.Users.Should().HaveCount(0);
            await testEventBroker.Object.Command(new CreateUserCommand(user));
            userEventHandler.Users.Should().HaveCount(1);
            userEventHandler.Users.FirstOrDefault().Should().Be(user);

            return user;
        }

        private async Task<List<UserDto>> QueryUsers(string userEmail, Mock<IEventBroker> testEventBroker)
        {
            var result = await testEventBroker.Object.Query<UserQuery, List<UserDto>>(new UserQuery(userEmail));
            result.Should().HaveCount(1);
            result.FirstOrDefault().Email.Should().Be(userEmail);
            return result;
        }

        private void StopUserEventHandler(UserEventHandler userEventHandler, Mock<IEventBroker> testEventBroker)
        {
            userEventHandler.Stop();
            testEventBroker.Verify(t => t.Unsubscribe(It.IsAny<Func<CreateUserCommand, Task>>()), Times.Once);
            testEventBroker.Verify(t => t.Unsubscribe(It.IsAny<Func<UserQuery, Task<List<UserDto>>>>()), Times.Once);
        }

        private async Task DoNotCreateUser(UserEventHandler userEventHandler, Mock<IEventBroker> testEventBroker)
        {
            var newUser = new UserDto { Email = "another@user.com" };

            userEventHandler.Users.Should().HaveCount(1);
            await testEventBroker.Object.Command(new CreateUserCommand(newUser));
            userEventHandler.Users.Should().HaveCount(1);
        }

        private async Task QueryNullUsers(string userEmail, Mock<IEventBroker> testEventBroker)
        {
            var result = await testEventBroker.Object.Query<UserQuery, List<UserDto>>(new UserQuery(userEmail));
            result.Should().BeNull();
        }
    }
}
