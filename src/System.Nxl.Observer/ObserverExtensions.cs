using Microsoft.Extensions.DependencyInjection;

namespace System.Nxl.Observer
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions to inject an <see cref="IEventBroker"/>.
    /// </summary>
    public static class ObserverExtensions
    {
        /// <summary>
        /// Adds an instance of <see cref="IEventBroker"/> to the service collection.
        /// </summary>
        /// <param name="services">
        ///     Service collection to be injected with the instance of <see cref="IEventBroker"/>.
        /// </param>
        /// <param name="options">Options to configure the instance.</param>
        /// <returns>Injected <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddObserver(
            this IServiceCollection services,
            Action<ObserverOptions> options = null)
        {
            var optionsInstance = new ObserverOptions();
            var eventBroker = ObserverBuilder.Build(optionsInstance, options);
            services.AddSingleton(eventBroker);
            return services;
        }
    }
}
