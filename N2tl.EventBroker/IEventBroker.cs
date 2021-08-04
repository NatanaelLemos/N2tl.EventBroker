using System;
using System.Threading.Tasks;

namespace N2tl.EventBroker
{
    /// <summary>
    /// Interface for subscribing and notifying events into the EventBroker.
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
        void SubscribeEvent<TEvent>(Func<TEvent, Task> callback);

        /// <summary>
        /// Subscribes to a result of <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="callback">Function to call containing the result of <typeparamref name="TEvent"/>.</param>
        /// <typeparam name="TEvent">Type of the event to subscribe to.</typeparam>
        /// <typeparam name="TResult">Type of result expected from the event.</typeparam>
        void SubscribeResult<TEvent, TResult>(Func<TEvent, TResult, Task> callback);

        /// <summary>
        /// Unsubscribe from an event <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to be unsubscribed from.</typeparam>
        /// <param name="callback">Function that will be unsubscribed.</param>
        void UnsubscribeEvent<TEvent>(Func<TEvent, Task> callback);

        /// <summary>
        /// Unsubscribe from an event <typeparamref name="TEvent"/> with result <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="callback">Function that will be unsubscribed.</param>
        /// <typeparam name="TEvent">Type of event to unsubscribe from.</typeparam>
        /// <typeparam name="TResult">Type of result to unsubscribe from.</typeparam>
        void UnsubscribeResult<TEvent, TResult>(Func<TEvent, TResult, Task> callback);

        /// <summary>
        /// Notifies that <typeparamref name="TEvent"/> happened.
        /// </summary>
        /// <typeparam name="TEvent">Event type to be notified.</typeparam>
        /// <param name="evt">Instance of <typeparamref name="TEvent"/>.</param>
        /// <returns>Task indicating the notification.</returns>
        Task SendEvent<TEvent>(TEvent evt);

        /// <summary>
        /// Notifies that the result <typeparamref name="TResult"/> is ready.
        /// </summary>
        /// <param name="evt">The event that originated that result.</param>
        /// <param name="result">The result for the command.</param>
        /// <typeparam name="TEvent">Type of event that originated the result.</typeparam>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <returns>Task indicating the notification.</returns>
        Task SendResult<TEvent, TResult>(TEvent evt, TResult result);
    }
}
