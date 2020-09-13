using System;
using System.Collections.Generic;
using System.Text;

namespace N2tl.Observer.IntegrationTests.Events.Users
{
    public class UserQuery
    {
        public UserQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}
