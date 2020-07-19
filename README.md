# System.Nxl.Observer

Implementation of the Observer pattern in the form of an EventBroker.

## Usage

To inject it to your dependency container, follow:

### With IServiceCollection

``` C#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddObserver();
        }
```

### Without IServiceCollection

``` C#
   IEventBroker eventBroker = ObserverBuilder.Build();
```

---
Optionally, the ObserverBuilder can be configured with interrupters.

Once interrupters are added to the pipeline, events will only go through if the condition in any interrupter returns true.

### ObserverBuilder

``` C#
        public void ConfigureServices(IServiceCollection services)
        {
            Func<Type, Task<bool>> interrupter = t => Task.FromResult(true);

            services.AddObserver(opt => opt.AddInterrupter(interrupter));
        }

        or

        Func<Type, Task<bool>> interrupter = t => Task.FromResult(true);
        ObserverBuilder.Build(opt => opt.AddInterrupter(interrupter));
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
