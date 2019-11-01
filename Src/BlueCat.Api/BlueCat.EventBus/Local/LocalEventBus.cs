

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlueCat.Extensions;

namespace BlueCat.EventBus.Local
{
    /// <summary>
    /// Implements EventBus as Singleton pattern.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.Local.EventBusBase" />
    /// <seealso cref="BlueCat.EventBus.Local.ILocalEventBus" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    //[ExposeServices(typeof(ILocalEventBus), typeof(LocalEventBus))]
    public class LocalEventBus : EventBusBase, ILocalEventBus
        //, ISingletonDependency
    {
        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public ILogger<LocalEventBus> Logger { get; set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected LocalEventBusOptions Options { get; }

        /// <summary>
        /// Gets the handler factories.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }

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
        /// Initializes a new instance of the <see cref="LocalEventBus" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public LocalEventBus(
            IOptions<LocalEventBusOptions> options,
            IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Logger = NullLogger<LocalEventBus>.Instance;

            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            Subscribe(Options.Handlers);
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
        /// Subscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                    factories.Add(factory)
                );

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        /// <summary>
        /// Unsubscribes the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="action">The action.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <summary>
        /// Unsubscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                    );
                });
        }

        /// <summary>
        /// Unsubscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="factory">The factory.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <summary>
        /// Unsubscribes all.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        /// <summary>
        /// publish as an asynchronous operation.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <exception cref="AggregateException">More than one error has occurred while triggering the event: " + eventType</exception>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public override async Task PublishAsync(Type eventType, object eventData)
        {
            var exceptions = new List<Exception>();

            await TriggerHandlersAsync(eventType, eventData, exceptions);

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    throw exceptions[0];
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        /// <summary>
        /// Gets the handler factories.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>IEnumerable&lt;EventTypeWithEventHandlerFactories&gt;.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        /// <summary>
        /// Gets the or create handler factories.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>List&lt;IEventHandlerFactory&gt;.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        /// <summary>
        /// Shoulds the trigger event for handler.
        /// </summary>
        /// <param name="targetEventType">Type of the target event.</param>
        /// <param name="handlerEventType">Type of the handler event.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }
    }
}