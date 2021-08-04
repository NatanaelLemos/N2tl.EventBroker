namespace N2tl.EventBroker.IntegrationTests.Events
{
    public class GetUserCommand
    {
        public GetUserCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
