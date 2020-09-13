using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N2tl.Observer.IntegrationTests.Events.Users
{
    public class UserEventHandler
    {
        private readonly IEventBroker _eventBroker;

        public UserEventHandler(IEventBroker eventBroker)
        {
            _eventBroker = eventBroker;
            Users = new List<UserDto>();
        }

        public List<UserDto> Users { get; }

        public void Start()
        {
            _eventBroker.Subscribe<CreateUserCommand>(CreateUserCommandHandler);
            _eventBroker.Subscribe<UserQuery, List<UserDto>>(UserQueryHandler);
        }

        public void Stop()
        {
            _eventBroker.Unsubscribe<CreateUserCommand>(CreateUserCommandHandler);
            _eventBroker.Unsubscribe<UserQuery, List<UserDto>>(UserQueryHandler);
        }

        private Task CreateUserCommandHandler(CreateUserCommand command)
        {
            Users.Add(command.User);
            return Task.CompletedTask;
        }

        private Task<List<UserDto>> UserQueryHandler(UserQuery query)
        {
            return Task.FromResult(Users.Where(u => u.Email == query.Email).ToList());
        }
    }
}
