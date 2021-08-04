using System;

namespace N2tl.EventBroker
{
    /// <summary>
    /// Builder for <see cref="IEventBroker"/>.
    /// </summary>
    public static class EventBrokerBuilder
    {
        /// <summary>
        /// Builds an instance of <see cref="IEventBroker"/>.
        /// </summary>
        /// <param name="options">Options to configure the instance.</param>
        /// <returns>Instance of <see cref="IEventBroker"/>.</returns>
        public static IEventBroker Build(Action<EventBrokerOptions> options = null)
        {
            var optionsInstance = new EventBrokerOptions();
            return Build(optionsInstance, options);
        }

        internal static IEventBroker Build(
            EventBrokerOptions optionsInstance,
            Action<EventBrokerOptions> options)
        {
            options?.Invoke(optionsInstance);
            var eventBroker = new EventBroker(optionsInstance.Interrupters);
            return eventBroker;
        }
    }
}
