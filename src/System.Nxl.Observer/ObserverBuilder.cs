namespace System.Nxl.Observer
{
    /// <summary>
    /// Builder for <see cref="IEventBroker"/>.
    /// </summary>
    public static class ObserverBuilder
    {
        /// <summary>
        /// Builds an instance of <see cref="IEventBroker"/>.
        /// </summary>
        /// <param name="options">Options to configure the instance.</param>
        /// <returns>Instance of <see cref="IEventBroker"/>.</returns>
        public static IEventBroker Build(Action<ObserverOptions> options = null)
        {
            var optionsInstance = new ObserverOptions();
            return Build(optionsInstance, options);
        }

        internal static IEventBroker Build(
            ObserverOptions optionsInstance,
            Action<ObserverOptions> options)
        {
            options?.Invoke(optionsInstance);
            var eventBroker = new EventBroker(optionsInstance.Interrupters);
            return eventBroker;
        }
    }
}
