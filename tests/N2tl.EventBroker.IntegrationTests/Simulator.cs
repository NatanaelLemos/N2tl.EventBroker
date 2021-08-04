using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using N2tl.EventBroker.IntegrationTests.Dtos;
using N2tl.EventBroker.IntegrationTests.Events;
using Xunit;

namespace N2tl.EventBroker.IntegrationTests
{
    public class Simulator : IDisposable
    {
        private const string _invalidName = "invalidName";
        private readonly UserDto _result = new UserDto { Name = "name", Email = "email" };

        private bool _allowToPass = false;

        private IEventBroker _eventBroker;
        private List<UserDto> _users;

        public Simulator()
        {
            _eventBroker = EventBrokerBuilder.Build(opt => opt
                .AddGeneralInterrupter(_ => Task.FromResult(_allowToPass))
                .AddEventInterrupter((GetUserCommand command) => Task.FromResult(command.Name != _invalidName)));

            _users = new List<UserDto>();
        }

        public void Dispose()
        {
            _eventBroker.UnsubscribeEvent<GetUserCommand>(GetUserCommandHandler);
            _eventBroker.UnsubscribeResult<GetUserCommand, UserDto>(GetUserResultHandler);
            _eventBroker.Dispose();
        }

        [Fact]
        public async Task SendAndReceiveEvents()
        {
            _eventBroker.SubscribeEvent<GetUserCommand>(GetUserCommandHandler);
            _eventBroker.SubscribeResult<GetUserCommand, UserDto>(GetUserResultHandler);

            _allowToPass = false;
            await _eventBroker.SendEvent(new GetUserCommand("test"));
            _users.Should().HaveCount(0, "allowToPass being false should interrupt the call.");

            _allowToPass = true;
            await _eventBroker.SendEvent(new GetUserCommand("test"));
            _users.Should().HaveCount(1, "allowToPass is now true, so it should pass.");

            await _eventBroker.SendEvent(new GetUserCommand(_invalidName));
            _users.Should().HaveCount(1, "invalidName should filter dto out.");
        }

        private Task GetUserCommandHandler(GetUserCommand command)
        {
            return _eventBroker.SendResult(command, _result);
        }

        private Task GetUserResultHandler(GetUserCommand command, UserDto dto)
        {
            _users.Add(dto);
            return Task.CompletedTask;
        }
    }
}
