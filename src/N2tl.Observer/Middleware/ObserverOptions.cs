using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace N2tl.Observer
{
    /// <summary>
    /// Options to configure the instance of <see cref="IEventBroker"/>.
    /// </summary>
    public class ObserverOptions
    {
        private readonly List<object> _interrupters = new List<object>();

        internal object[] Interrupters => _interrupters.ToArray();

        /// <summary>
        /// Adds interrupters to the event pipeline.
        /// Once interrupters are added, events will only go through if the condition in any interrupter returns true.
        /// </summary>
        /// <param name="interrupter">An interrupter function.</param>
        /// <typeparam name="TEvent">The event type to be checked against.</typeparam>
        /// <returns>Options instance with the interrupter injected.</returns>
        public ObserverOptions AddEventInterrupter<TEvent>(Func<TEvent, Task<bool>> interrupter)
        {
            _interrupters.Add(interrupter);
            return this;
        }

        public ObserverOptions AddGeneralInterrupter(Func<object, Task<bool>> interrupter)
        {
            _interrupters.Add(interrupter);
            return this;
        }
    }
}
