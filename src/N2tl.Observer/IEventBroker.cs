using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// Interface for subscribing and notifying events into the Observer.
    /// </summary>
    public interface IEventBroker : IDisposable
    {
        /// <summary>
        /// Subscribes to an event <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to be subscribed to.</typeparam>
        /// <param name="callback">
        ///     Function to be called when
        ///     <typeparamref name="TEvent"/> happens.
        /// </param>
        void Subscribe<TEvent>(Func<TEvent, Task> callback);

        /// <summary>
        /// Unsubscribe from an event <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to be unsubscribed from.</typeparam>
        /// <param name="callback">Function that will be unsubscribed.</param>
        void Unsubscribe<TEvent>(Func<TEvent, Task> callback);

        /// <summary>
        /// Notifies that <typeparamref name="TEvent"/> happened.
        /// </summary>
        /// <typeparam name="TEvent">Event type to be notified.</typeparam>
        /// <param name="command">Instance of <typeparamref name="TEvent"/>.</param>
        /// <returns>Task indicating the notification.</returns>
        Task Notify<TEvent>(TEvent command);
    }
}
