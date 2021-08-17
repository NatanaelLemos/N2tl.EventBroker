using System;
using System.Threading.Tasks;

namespace N2tl.EventBroker
{
    internal class EventNotification<TEvent> : IDisposable
    {
        private event Func<TEvent, Task> OnNewNotification;

        public void Subscribe(Func<TEvent, Task> callback)
        {
            OnNewNotification += callback;
        }

        public void Unsubscribe(Func<TEvent, Task> callback)
        {
            if (OnNewNotification == null)
            {
                return;
            }

            OnNewNotification -= callback;
        }

        public Task Notify(TEvent message)
        {
            if (OnNewNotification == null)
            {
                return Task.CompletedTask;
            }

            return OnNewNotification(message);
        }

        public void Dispose()
        {
            if (OnNewNotification == null)
            {
                return;
            }

            foreach (var handler in OnNewNotification.GetInvocationList())
            {
                OnNewNotification -= (Func<TEvent, Task>)handler;
            }

            OnNewNotification = null;
        }
    }
}
