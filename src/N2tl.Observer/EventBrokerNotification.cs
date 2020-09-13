using System;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    internal class EventBrokerNotification<T> : IDisposable
    {
        private event Func<T, Task> OnNewNotification;

        public void Subscribe(Func<T, Task> callback)
        {
            OnNewNotification += callback;
        }

        public void Unsubscribe(Func<T, Task> callback)
        {
            if (OnNewNotification == null)
            {
                return;
            }

            OnNewNotification -= callback;
        }

        public Task Notify(T message)
        {
            if (OnNewNotification == null)
            {
                return null;
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
                OnNewNotification -= (Func<T, Task>)handler;
            }

            OnNewNotification = null;
        }
    }
}
