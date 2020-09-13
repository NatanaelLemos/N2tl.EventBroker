using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    internal partial class EventBroker : IEventBroker
    {
        /// <inheritdoc />
        public void Subscribe<TQuery, TQueryResult>(Func<TQuery, Task<TQueryResult>> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TQuery>();
            eventNotification.Subscribe(callback);
        }

        /// <inheritdoc />
        public void Unsubscribe<TQuery, TQueryResult>(Func<TQuery, Task<TQueryResult>> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TQuery>();
            eventNotification.Unsubscribe(callback);
        }

        public async Task<TQueryResult> Query<TQuery, TQueryResult>(TQuery query)
        {
            if (query == null)
            {
                return default;
            }

            if (await CommandWasInterrupted(query))
            {
                return default;
            }

            var eventNotification = GetEventNotification<TQuery>();
            var task = eventNotification.Notify(query);
            if (task == null)
            {
                return default;
            }

            return await ((Task<TQueryResult>)task);
        }
    }
}
