using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace N2tl.Observer.UnitTests
{
    public class QueryBrokerTests
    {
        private static List<Person> _people = new List<Person>
        {
            new Person{Name="John Doe"}
        };

        [Fact]
        public async Task QueryShouldReturnExpectedValue()
        {
            var eventBroker = new EventBroker();
            eventBroker.Subscribe<QueryPeople, List<Person>>(QueryPeopleHandler);
            var result = await eventBroker.Query<QueryPeople, List<Person>>(new QueryPeople());
            result.Should().BeEquivalentTo(_people);
        }

        private Task<List<Person>> QueryPeopleHandler(QueryPeople query)
        {
            return Task.FromResult(_people);
        }

        private class QueryPeople
        {
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}
