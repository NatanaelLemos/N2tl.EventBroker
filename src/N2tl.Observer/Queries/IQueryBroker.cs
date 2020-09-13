using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// Interface for subscribing and notifying events into the Observer.
    /// </summary>
    public interface IQueryBroker
    {
        /// <summary>
        /// Subscribes to an event <typeparamref name="TQuery"/>.
        /// </summary>
        /// <typeparam name="TQuery">The type of event to be subscribed to.</typeparam>
        /// <typeparam name="TQueryResult">The type of result it should return.</typeparam>
        /// <param name="callback">
        ///     Function to be called when
        ///     <typeparamref name="TQuery"/> happens.
        /// </param>
        void Subscribe<TQuery, TQueryResult>(Func<TQuery, Task<TQueryResult>> callback);

        /// <summary>
        /// Unsubscribe from an event <typeparamref name="TQuery"/>.
        /// </summary>
        /// <typeparam name="TQuery">Type of event to be unsubscribed from.</typeparam>
        /// <param name="callback">Function that will be unsubscribed.</param>
        void Unsubscribe<TQuery, TQueryResult>(Func<TQuery, Task<TQueryResult>> callback);

        /// <summary>
        /// Notifies that <typeparamref name="TQuery"/> happened.
        /// </summary>
        /// <typeparam name="TQuery">Event type to be notified.</typeparam>
        /// <typeparam name="TQueryResult">Result to be returned.</typeparam>
        /// <param name="query">Instance of <typeparamref name="TQuery"/>.</param>
        /// <returns>Task containing the result.</returns>
        Task<TQueryResult> Query<TQuery, TQueryResult>(TQuery query);
    }
}
