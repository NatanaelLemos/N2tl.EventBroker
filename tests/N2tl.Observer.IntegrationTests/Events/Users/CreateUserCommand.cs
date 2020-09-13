using System;
using System.Collections.Generic;
using System.Text;

namespace N2tl.Observer.IntegrationTests.Events.Users
{
    public class CreateUserCommand
    {
        public CreateUserCommand(UserDto user)
        {
            User = user;
        }

        public UserDto User { get; }
    }
}
