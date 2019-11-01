
using BlueCat.EventBus;
using BlueCat.EventBus.Distributed;
using BlueCat.EventBus.Local;
using BlueCat.Extensions;
using BlueCat.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace BlueCat.EventBus.Distributed.RabbitMq
{
    /* TODO: How to handle unsubscribe to unbind on RabbitMq (may not be possible for)
     * TODO: Implement Retry system
     * TODO: Should be improved
     */
    /// <summary>
    /// Class RabbitMqDistributedEventBus.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.Local.EventBusBase" />
    /// <seealso cref="BlueCat.EventBus.Distributed.IDistributedEventBus" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class RabbitMqDistributedEventBus : EventBusBase, IDistributedEventBus
    {
        /// <summary>
        /// Gets the rabbit mq distributed event bus options.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected RabbitMqDistributedEventBusOptions RabbitMqDistributedEventBusOptions { get; }
        /// <summary>
        /// Gets the distributed event bus options.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected DistributedEventBusOptions DistributedEventBusOptions { get; }
        /// <summary>
        /// Gets the connection pool.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected IConnectionPool ConnectionPool { get; }
        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected IRabbitMqSerializer Serializer { get; }
        /// <summary>
        /// Gets the handler factories.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; } //TODO: Accessing to the List<IEventHandlerFactory> may not be thread-safe!
        /// <summary>
        /// Gets the event types.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected ConcurrentDictionary<string, Type> EventTypes { get; }
        /// <summary>
        /// The consumer channel
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected IModel ConsumerChannel;
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqDistributedEventBus"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="connectionPool">The connection pool.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="distributedEventBusOptions">The distributed event bus options.</param>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public RabbitMqDistributedEventBus(
            IOptions<RabbitMqDistributedEventBusOptions> options,
            IConnectionPool connectionPool,
            IRabbitMqSerializer serializer,
            IServiceProvider serviceProvider, 
            IOptions<DistributedEventBusOptions> distributedEventBusOptions)
        {
            ConnectionPool = connectionPool;
            Serializer = serializer;
            ServiceProvider = serviceProvider;
            DistributedEventBusOptions = distributedEventBusOptions.Value;
            RabbitMqDistributedEventBusOptions = options.Value;
            
            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            EventTypes = new ConcurrentDictionary<string, Type>();

            ConsumerChannel = CreateConsumerChannel();
            Subscribe(DistributedEventBusOptions.Handlers);
        }

        /// <summary>
        /// Subscribes the specified handlers.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected virtual void Subscribe(ITypeList<IEventHandler> handlers)
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
        /// Creates the consumer channel.
        /// </summary>
        /// <returns>IModel.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        private IModel CreateConsumerChannel()
        {
            //TODO: Support multiple connection (and consumer)?
            var channel = ConnectionPool.Get().CreateModel();
            channel.ExchangeDeclare(
                exchange: RabbitMqDistributedEventBusOptions.ExchangeName,
                type: "direct"
            );

            channel.QueueDeclare(
                queue: RabbitMqDistributedEventBusOptions.ClientName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) => { await ProcessEventAsync(channel, ea); };

            channel.BasicConsume(
                queue: RabbitMqDistributedEventBusOptions.ClientName,
                autoAck: false,
                consumer: consumer
            );

            channel.CallbackException += (sender, ea) =>
            {
                ConsumerChannel.Dispose();
                ConsumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        /// <summary>
        /// process event as an asynchronous operation.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="ea">The <see cref="BasicDeliverEventArgs"/> instance containing the event data.</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;
            var eventType = EventTypes.GetValueOrDefault(eventName);
            if (eventType == null)
            {
                return;
            }

            var eventData = Serializer.Deserialize(ea.Body, eventType);

            await TriggerHandlersAsync(eventType, eventData);

            channel.BasicAck(ea.DeliveryTag, multiple: false);
        }

        /// <summary>
        /// Subscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            var handlerFactories = GetOrCreateHandlerFactories(eventType);
            
            handlerFactories.Add(factory);

            if (handlerFactories.Count == 1) //TODO: Multi-threading!
            {
                var eventName = EventNameAttribute.GetNameOrDefault(eventType);

                using (var channel = ConnectionPool.Get().CreateModel()) //TODO: Connection name per event!
                {
                    channel.QueueBind(
                        queue: RabbitMqDistributedEventBusOptions.ClientName,
                        exchange: RabbitMqDistributedEventBusOptions.ExchangeName,
                        routingKey: eventName
                    );
                }
            }

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        /// <summary>
        /// Unsubscribes the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="action">The action.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            Check.NotNull(action, nameof(action));

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
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
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
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
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
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public override Task PublishAsync(Type eventType, object eventData)
        {
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            var body = Serializer.Serialize(eventData);

            using (var channel = ConnectionPool.Get().CreateModel()) //TODO: Connection name per event!
            {
                //TODO: Other properties like durable?
                channel.ExchangeDeclare(RabbitMqDistributedEventBusOptions.ExchangeName, "direct");
                
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; //persistent

                channel.BasicPublish(
                   exchange: RabbitMqDistributedEventBusOptions.ExchangeName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the or create handler factories.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>List&lt;IEventHandlerFactory&gt;.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    var eventName = EventNameAttribute.GetNameOrDefault(type);
                    EventTypes[eventName] = type;
                    return new List<IEventHandlerFactory>();
                }
            );
        }

        /// <summary>
        /// Gets the handler factories.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>IEnumerable&lt;EventTypeWithEventHandlerFactories&gt;.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
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
        /// Shoulds the trigger event for handler.
        /// </summary>
        /// <param name="targetEventType">Type of the target event.</param>
        /// <param name="handlerEventType">Type of the handler event.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }
    }
}