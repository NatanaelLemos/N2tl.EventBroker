using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Nxl.Observer
{
    /// <summary>
    /// Options to configure the instance of <see cref="IEventBroker"/>.
    /// </summary>
    public class ObserverOptions
    {
        private readonly List<Func<Type, Task<bool>>> _interrupters = new List<Func<Type, Task<bool>>>();

        internal Func<Type, Task<bool>>[] Interrupters => _interrupters.ToArray();

        /// <summary>
        /// Adds interrupters to the event pipeline.
        /// Once interrupters are added, events will only go through if the condition in any interrupter returns true.
        /// </summary>
        /// <param name="interrupter">An interrupter function.</param>
        /// <returns>Options instance with the interrupter injected.</returns>
        public ObserverOptions AddInterrupter(Func<Type, Task<bool>> interrupter)
        {
            _interrupters.Add(interrupter);
            return this;
        }
    }
}
