using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// An internal implementation of <see cref="IEventBroker"/>.
    /// </summary>
    internal partial class EventBroker : IEventBroker
    {
        private readonly Dictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>();
        private readonly List<object> _interrupters = new List<object>();

        /// <summary>
        /// Creates an instance of <see cref="EventBroker"/>.
        /// </summary>
        /// <param name="interrupters">
        ///     List of functions that can interrupt the execution of an event.
        /// </param>
        internal EventBroker(params object[] interrupters)
        {
            if (interrupters != null)
            {
                _interrupters = interrupters.ToList();
            }
        }

        private EventBrokerNotification<TCommand> GetEventNotification<TCommand>()
        {
            var key = typeof(TCommand).FullName;
            EventBrokerNotification<TCommand> eventNotification = null;

            if (_subscriptions.ContainsKey(key))
            {
                eventNotification = _subscriptions[key] as EventBrokerNotification<TCommand>;
            }

            if (eventNotification == null)
            {
                eventNotification = new EventBrokerNotification<TCommand>();
                _subscriptions[key] = eventNotification;
            }

            return eventNotification;
        }

        private async Task<bool> CommandWasInterrupted<TEvent>(TEvent command)
        {
            if (await CommandWasInterruptedByTypeSpecificInterrupter(command))
            {
                return true;
            }

            if (await CommandWasInterruptedByGeneralInterrupter(command))
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

        private async Task<bool> CommandWasInterruptedByGeneralInterrupter<TEvent>(TEvent command)
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
