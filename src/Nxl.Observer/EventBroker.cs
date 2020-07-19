using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nxl.Observer
{
    /// <summary>
    /// An internal implementation of <see cref="IEventBroker"/>.
    /// </summary>
    internal class EventBroker : IEventBroker
    {
        private readonly ConcurrentDictionary<string, List<object>> _subscriptions;
        private readonly List<Func<Type, Task<bool>>> _interrupters;

        /// <summary>
        /// Creates an instance of <see cref="EventBroker"/>.
        /// </summary>
        /// <param name="interrupters">
        ///     List of functions that can interrupt the execution of an event.
        /// </param>
        public EventBroker(params Func<Type, Task<bool>>[] interrupters)
        {
            _subscriptions = new ConcurrentDictionary<string, List<object>>();
            _interrupters = interrupters == null
                ? new List<Func<Type, Task<bool>>>()
                : new List<Func<Type, Task<bool>>>(interrupters);
        }

        /// <inheritdoc />
        public void Subscribe<TEvent>(Func<TEvent, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var key = GetKey<TEvent>();
            if (_subscriptions.TryGetValue(key, out var listOfCallbacks))
            {
                listOfCallbacks.Add(callback);
            }
            else
            {
                _subscriptions.TryAdd(key, new List<object>
                {
                    callback
                });
            }
        }

        /// <inheritdoc />
        public async Task Notify<TEvent>(TEvent command)
        {
            if (command == null)
            {
                return;
            }

            var key = GetKey<TEvent>();
            if (!_subscriptions.TryGetValue(key, out var listOfCallbacks))
            {
                return;
            }

            foreach (var interrupter in _interrupters)
            {
                var isAllowed = await interrupter(typeof(TEvent));
                if (!isAllowed)
                {
                    return;
                }
            }

            var callbackResponses = listOfCallbacks.Select(c =>
            {
                var func = (Func<TEvent, Task>)c;
                return func(command);
            });

            await Task.WhenAll(callbackResponses);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _subscriptions.Clear();
            _interrupters.Clear();
        }

        private string GetKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }
    }
}
