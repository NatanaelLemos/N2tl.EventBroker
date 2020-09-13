using System;

namespace N2tl.Observer
{
    /// <summary>
    /// Interface for subscribing and notifying events into the Observer.
    /// </summary>
    public interface IEventBroker : ICommandBroker, IQueryBroker, IDisposable
    {
    }
}
