using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    internal partial class EventBroker : IEventBroker
    {
        /// <inheritdoc />
        public void Subscribe<TCommand>(Func<TCommand, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TCommand>();
            eventNotification.Subscribe(callback);
        }

        /// <inheritdoc />
        public void Unsubscribe<TCommand>(Func<TCommand, Task> callback)
        {
            if (callback == null)
            {
                return;
            }

            var eventNotification = GetEventNotification<TCommand>();
            eventNotification.Unsubscribe(callback);
        }

        /// <inheritdoc />
        public async Task Command<TCommand>(TCommand command)
        {
            if (command == null)
            {
                return;
            }

            if (await CommandWasInterrupted(command))
            {
                return;
            }

            var eventNotification = GetEventNotification<TCommand>();
            var task = eventNotification.Notify(command);
            if (task == null)
            {
                return;
            }

            await task;
        }
    }
}
