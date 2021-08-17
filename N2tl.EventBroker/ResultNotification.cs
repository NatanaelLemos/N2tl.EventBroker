using System;
using System.Threading.Tasks;

namespace N2tl.EventBroker
{
    internal class ResultNotification<TEvent, TResult> : IDisposable
    {
        private event Func<TEvent, TResult, Task> OnNewNotification;

        public void Subscribe(Func<TEvent, TResult, Task> callback)
        {
            OnNewNotification += callback;
        }

        public void Unsubscribe(Func<TEvent, TResult, Task> callback)
        {
            if (OnNewNotification == null)
            {
                return;
            }

            OnNewNotification -= callback;
        }

        public Task Notify(TEvent evt, TResult result)
        {
            if (OnNewNotification == null)
            {
                return Task.CompletedTask;
            }

            return OnNewNotification(evt, result);
        }

        public void Dispose()
        {
            if (OnNewNotification == null)
            {
                return;
            }

            foreach (var handler in OnNewNotification.GetInvocationList())
            {
                OnNewNotification -= (Func<TEvent, TResult, Task>)handler;
            }

            OnNewNotification = null;
        }
    }
}
