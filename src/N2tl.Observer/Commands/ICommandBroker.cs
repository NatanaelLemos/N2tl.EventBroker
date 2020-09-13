using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// Interface for subscribing and notifying events into the Observer.
    /// </summary>
    public interface ICommandBroker
    {
        /// <summary>
        /// Subscribes to an event <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">The type of event to be subscribed to.</typeparam>
        /// <param name="callback">
        ///     Function to be called when
        ///     <typeparamref name="TCommand"/> happens.
        /// </param>
        void Subscribe<TCommand>(Func<TCommand, Task> callback);

        /// <summary>
        /// Unsubscribe from an event <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of event to be unsubscribed from.</typeparam>
        /// <param name="callback">Function that will be unsubscribed.</param>
        void Unsubscribe<TCommand>(Func<TCommand, Task> callback);

        /// <summary>
        /// Notifies that <typeparamref name="TCommand"/> happened.
        /// </summary>
        /// <typeparam name="TCommand">Event type to be notified.</typeparam>
        /// <param name="command">Instance of <typeparamref name="TEvent"/>.</param>
        /// <returns>Task indicating the notification.</returns>
        Task Command<TCommand>(TCommand command);
    }
}
