using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N2tl.EventBroker
{
    /// <inheritdoc />
    internal class EventBroker : IEventBroker
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

        public void Dispose()
        {
            foreach (var subscription in _subscriptions.Values)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
            _interrupters.Clear();
        }

        /// <inheritdoc />
        public void SubscribeEvent<TEvent>(Func<TEvent, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TEvent>();
            eventNotification.Subscribe(callback);
        }

        /// <inheritdoc />
        public void SubscribeResult<TEvent, TResult>(Func<TEvent, TResult, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var resultNotification = GetResultNotification<TEvent, TResult>();
            resultNotification.Subscribe(callback);
        }

        /// <inheritdoc />
        public void UnsubscribeEvent<TEvent>(Func<TEvent, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TEvent>();
            eventNotification.Unsubscribe(callback);
        }

        /// <inheritdoc />
        public void UnsubscribeResult<TEvent, TResult>(Func<TEvent, TResult, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var resultNotification = GetResultNotification<TEvent, TResult>();
            resultNotification.Unsubscribe(callback);
        }

        /// <inheritdoc />
        public async Task SendEvent<TEvent>(TEvent evt)
        {
            if (evt == null)
            {
                return;
            }

            if (await CommandWasInterrupted(evt))
            {
                return;
            }

            var eventNotification = GetEventNotification<TEvent>();
            var task = eventNotification.Notify(evt);
            if (task == null)
            {
                return;
            }

            await task;
        }

        /// <inheritdoc />
        public async Task SendResult<TEvent, TResult>(TEvent evt, TResult result)
        {
            if (evt == null)
            {
                return;
            }

            var eventNotification = GetResultNotification<TEvent, TResult>();
            var task = eventNotification.Notify(evt, result);
            if (task == null)
            {
                return;
            }

            await task;
        }

        private EventNotification<TEvent> GetEventNotification<TEvent>()
        {
            var key = typeof(TEvent).FullName;
            EventNotification<TEvent> eventNotification = null;

            if (_subscriptions.ContainsKey(key))
            {
                eventNotification = _subscriptions[key] as EventNotification<TEvent>;
            }

            if (eventNotification == null)
            {
                eventNotification = new EventNotification<TEvent>();
                _subscriptions[key] = eventNotification;
            }

            return eventNotification;
        }

        private ResultNotification<TEvent, TResult> GetResultNotification<TEvent, TResult>()
        {
            var key = $"<{typeof(TEvent).FullName}>|<{typeof(TResult).FullName}>";
            ResultNotification<TEvent, TResult> resultNotification = null;

            if (_subscriptions.ContainsKey(key))
            {
                resultNotification = _subscriptions[key] as ResultNotification<TEvent, TResult>;
            }

            if (resultNotification == null)
            {
                resultNotification = new ResultNotification<TEvent, TResult>();
                _subscriptions[key] = resultNotification;
            }

            return resultNotification;
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
                .OfType<Func<TEvent, Task<bool>>>()
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
                .OfType<Func<object, Task<bool>>>()
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
    }
}
