using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// An internal implementation of <see cref="IEventBroker"/>.
    /// </summary>
    internal class EventBroker : IEventBroker
    {
        private readonly ConcurrentDictionary<string, List<object>> _subscriptions;
        private readonly List<object> _interrupters;

        /// <summary>
        /// Creates an instance of <see cref="EventBroker"/>.
        /// </summary>
        /// <param name="interrupters">
        ///     List of functions that can interrupt the execution of an event.
        /// </param>
        public EventBroker(params object[] interrupters)
        {
            _subscriptions = new ConcurrentDictionary<string, List<object>>();
            _interrupters = interrupters == null
                ? new List<object>()
                : interrupters.ToList();
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

            if(await CommandWasInterrupted(command))
            {
                return;
            }

            var callbackResponses = listOfCallbacks.Select(c =>
            {
                var func = (Func<TEvent, Task>)c;
                return func(command);
            });

            await Task.WhenAll(callbackResponses);
        }

        private async Task<bool> CommandWasInterrupted<TEvent>(TEvent command)
        {
            var typeSpecificInterrupters = _interrupters
                   .Where(i => i is Func<TEvent, Task<bool>>)
                   .Select(i => i as Func<TEvent, Task<bool>>)
                   .ToList();

            foreach (var interrupter in typeSpecificInterrupters)
            {
                var isAllowed = await interrupter(command);
                if (!isAllowed)
                {
                    return true;
                }
            }

            var generalInterrupters = _interrupters
                .Where(i => i is Func<object, Task<bool>>)
                .Select(i => i as Func<object, Task<bool>>)
                .ToList();

            foreach(var interrupter in generalInterrupters)
            {
                var isAllowed = await interrupter(command);
                if (!isAllowed)
                {
                    return true;
                }
            }

            return false;
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
