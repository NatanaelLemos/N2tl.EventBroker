# N2tl.EventBroker

Implementation of the Observer pattern in the form of an EventBroker.

You can find this project published at
https://www.nuget.org/packages/N2tl.EventBroker/

## Usage

To inject it to your dependency container, follow:

### With IServiceCollection

``` C#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEventBroker();
        }
```

### Without IServiceCollection

``` C#
   IEventBroker eventBroker = EventBrokerBuilder.Build();
```

---
Optionally, the EventBroker can be configured with interrupters.

Once interrupters are added to the pipeline, events will only go through if the condition in any interrupter returns true.

### ObserverBuilder with event specific interrupters.

``` C#
        public void ConfigureServices(IServiceCollection services)
        {
            Func<MyEventType, Task<bool>> interrupter = t => Task.FromResult(true);

            services.AddEventBroker(opt => opt.AddEventInterrupter(interrupter));
        }

        or

        Func<MyEventType, Task<bool>> interrupter = t => Task.FromResult(true);
        EventBrokerBuilder.Build(opt => opt.AddEventInterrupter(interrupter));
```

### ObserverBuilder with general interrupters.

``` C#
        public void ConfigureServices(IServiceCollection services)
        {
            Func<object, Task<bool>> interrupter = t => Task.FromResult(true);

            services.AddEventBroker(opt => opt.AddGeneralInterrupter(interrupter));
        }

        or

        Func<object, Task<bool>> interrupter = t => Task.FromResult(true);
        EventBrokerBuilder.Build(opt => opt.AddGeneralInterrupter(interrupter));
```

---
---
After injecting the observer, you'll have access to an instance of IEventBroker from which you can subscribe and notify events.

``` C#
private readonly IEventBroker _eventBroker;

public MyClass(IEventBroker eventBroker)
{
    _eventBroker = eventBroker;
    _eventBroker.Subscribe<MyEventType>(MyEventTypeHandler);
}

private Task MyEventTypeHandler(MyEventType arg)
{
    // Do whatever.
}

private async Task AnotherMethod()
{
    await _eventBroker.Notify(new MyEventType());
}
```
