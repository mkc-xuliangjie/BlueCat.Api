

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BlueCat.EventBus.Local;
using BlueCat.Extensions;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Class LocalDistributedEventBus.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.Distributed.IDistributedEventBus" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public class LocalDistributedEventBus : IDistributedEventBus
    {
        /// <summary>
        /// The local event bus
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        private readonly ILocalEventBus _localEventBus;
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected IServiceProvider ServiceProvider { get; }
        /// <summary>
        /// Gets the distributed event bus options.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected DistributedEventBusOptions DistributedEventBusOptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDistributedEventBus" /> class.
        /// </summary>
        /// <param name="localEventBus">The local event bus.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="distributedEventBusOptions">The distributed event bus options.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public LocalDistributedEventBus(
            ILocalEventBus localEventBus, 
            IServiceProvider serviceProvider,
            IOptions<DistributedEventBusOptions> distributedEventBusOptions)
        {
            _localEventBus = localEventBus;
            ServiceProvider = serviceProvider;
            DistributedEventBusOptions = distributedEventBusOptions.Value;
            Subscribe(distributedEventBusOptions.Value.Handlers);
        }

        /// <summary>
        /// Subscribes the specified handlers.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual void Subscribe(ITypeList<IEventHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                var interfaces = handler.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                    {
                        continue;
                    }

                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        Subscribe(genericArgs[0], new IocEventHandlerFactory(ServiceProvider, handler));
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return _localEventBus.Subscribe(action);
        }

        /// <summary>
        /// Subscribes the specified handler.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
        {
            return _localEventBus.Subscribe(handler);
        }

        /// <summary>
        /// Subscribes this instance.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <typeparam name="THandler">The type of the t handler.</typeparam>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler, new()
        {
            return _localEventBus.Subscribe<TEvent, THandler>();
        }

        /// <summary>
        /// Subscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return _localEventBus.Subscribe(eventType, handler);
        }

        /// <summary>
        /// Subscribes the specified factory.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return _localEventBus.Subscribe<TEvent>(factory);
        }

        /// <summary>
        /// Subscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            return _localEventBus.Subscribe(eventType, factory);
        }

        /// <summary>
        /// Unsubscribes the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="action">The action.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            _localEventBus.Unsubscribe(action);
        }

        /// <summary>
        /// Unsubscribes the specified handler.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
        {
            _localEventBus.Unsubscribe(handler);
        }

        /// <summary>
        /// Unsubscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            _localEventBus.Unsubscribe(eventType, handler);
        }

        /// <summary>
        /// Unsubscribes the specified factory.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            _localEventBus.Unsubscribe<TEvent>(factory);
        }

        /// <summary>
        /// Unsubscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="factory">The factory.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            _localEventBus.Unsubscribe(eventType, factory);
        }

        /// <summary>
        /// Unsubscribes all.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            _localEventBus.UnsubscribeAll<TEvent>();
        }

        /// <summary>
        /// Unsubscribes all.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void UnsubscribeAll(Type eventType)
        {
            _localEventBus.UnsubscribeAll(eventType);
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public Task PublishAsync<TEvent>(TEvent eventData)
            where TEvent : class
        {
            return _localEventBus.PublishAsync(eventData);
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public Task PublishAsync(Type eventType, object eventData)
        {
            return _localEventBus.PublishAsync(eventType, eventData);
        }
    }
}