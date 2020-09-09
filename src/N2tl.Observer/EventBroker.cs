using System;
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
        private readonly Dictionary<string, IDisposable> _subscriptions;
        private readonly List<object> _interrupters;

        /// <summary>
        /// Creates an instance of <see cref="EventBroker"/>.
        /// </summary>
        /// <param name="interrupters">
        ///     List of functions that can interrupt the execution of an event.
        /// </param>
        internal EventBroker(params object[] interrupters)
        {
            _subscriptions = new Dictionary<string, IDisposable>();
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

            var eventNotification = GetEventNotification<TEvent>();
            eventNotification.Subscribe(callback);
        }

        /// <inheritdoc />
        public void Unsubscribe<TEvent>(Func<TEvent, Task> callback)
        {
            if(callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TEvent>();
            eventNotification.Unsubscribe(callback);
        }

        /// <inheritdoc />
        public async Task Notify<TEvent>(TEvent command)
        {
            if (command == null)
            {
                return;
            }

            if (await CommandWasInterrupted(command))
            {
                return;
            }

            var eventNotification = GetEventNotification<TEvent>();
            await eventNotification.Notify(command);
        }

        private EventBrokerNotification<TEvent> GetEventNotification<TEvent>()
        {
            var key = typeof(TEvent).FullName;
            EventBrokerNotification<TEvent> eventNotification = null;

            if (_subscriptions.ContainsKey(key))
            {
                eventNotification = _subscriptions[key] as EventBrokerNotification<TEvent>;
            }

            if (eventNotification == null)
            {
                eventNotification = new EventBrokerNotification<TEvent>();
                _subscriptions[key] = eventNotification;
            }

            return eventNotification;
        }

        private async Task<bool> CommandWasInterrupted<TEvent>(TEvent command)
        {
            if(await CommandWasInterruptedByTypeSpecificInterrupter(command))
            {
                return true;
            }

            if(await CommandWasInterruptedByGeneralInterrupter(command))
            {
                return true;
            }

            return false;
        }

        private async Task<bool> CommandWasInterruptedByTypeSpecificInterrupter<TEvent>(TEvent command)
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

            return false;
        }

        private async Task<bool>CommandWasInterruptedByGeneralInterrupter<TEvent>(TEvent command)
        {
            var generalInterrupters = _interrupters
                .Where(i => i is Func<object, Task<bool>>)
                .Select(i => i as Func<object, Task<bool>>)
                .ToList();

            foreach (var interrupter in generalInterrupters)
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
            foreach (var subscription in _subscriptions.Values)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
            _interrupters.Clear();
        }
    }
}
