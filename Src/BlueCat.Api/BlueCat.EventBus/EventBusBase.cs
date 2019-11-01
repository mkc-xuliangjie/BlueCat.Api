using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using BlueCat.EventBus.Distributed;
using BlueCat.Extensions.Reflection;

namespace BlueCat.EventBus.Local
{
    /// <summary>
    /// Class EventBusBase.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventBus" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public abstract class EventBusBase : IEventBus
    {
        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return Subscribe(typeof(TEvent), new ActionEventHandler<TEvent>(action));
        }

        /// <summary>
        /// Subscribes the specified handler.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }

        /// <summary>
        /// Subscribes this instance.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <typeparam name="THandler">The type of the t handler.</typeparam>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual IDisposable Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler, new()
        {
            return Subscribe(typeof(TEvent), new TransientEventHandlerFactory<THandler>());
        }

        /// <summary>
        /// Subscribes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return Subscribe(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <summary>
        /// Subscribes the specified factory.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns>IDisposable.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return Subscribe(typeof(TEvent), factory);
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
        public abstract IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);

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
        public abstract void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;

        /// <summary>
        /// Unsubscribes the specified handler.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
        {
            Unsubscribe(typeof(TEvent), handler);
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
        public abstract void Unsubscribe(Type eventType, IEventHandler handler);

        /// <summary>
        /// Unsubscribes the specified factory.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            Unsubscribe(typeof(TEvent), factory);
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
        public abstract void Unsubscribe(Type eventType, IEventHandlerFactory factory);

        /// <summary>
        /// Unsubscribes all.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual void UnsubscribeAll<TEvent>() where TEvent : class
        {
            UnsubscribeAll(typeof(TEvent));
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
        public abstract void UnsubscribeAll(Type eventType);

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class
        {
            return PublishAsync(typeof(TEvent), eventData);
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public abstract Task PublishAsync(Type eventType, object eventData);

        /// <summary>
        /// trigger handlers as an asynchronous operation.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>Task.</returns>
        /// <exception cref="AggregateException">More than one error has occurred while triggering the event: " + eventType</exception>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual async Task TriggerHandlersAsync(Type eventType, object eventData)
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
        /// trigger handlers as an asynchronous operation.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected virtual async Task TriggerHandlersAsync(Type eventType, object eventData, List<Exception> exceptions)
        {
            await new SynchronizationContextRemover();

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories.ToArray()) //TODO: ToArray should not be needed!
                {
                    await TriggerHandlerAsync(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = Activator.CreateInstance(baseEventType, constructorArgs);
                    await PublishAsync(baseEventType, baseEventData);
                }
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
        protected abstract IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType);

        /// <summary>
        /// trigger handler as an asynchronous operation.
        /// </summary>
        /// <param name="asyncHandlerFactory">The asynchronous handler factory.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>Task.</returns>
        /// <exception cref="Exception">The object instance is not an event handler. Object type: " + handlerType.AssemblyQualifiedName</exception>
        /// <exception cref="System.Exception">The object instance is not an event handler. Object type: " + handlerType.AssemblyQualifiedName</exception>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected virtual async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType, object eventData, List<Exception> exceptions)
        {
            using (var eventHandlerWrapper = asyncHandlerFactory.GetHandler())
            {
                try
                {
                    var handlerType = eventHandlerWrapper.EventHandler.GetType();

                    if (ReflectionHelper.IsAssignableToGenericType(handlerType, typeof(IEventHandler<>)))
                    {
                        var method = typeof(IEventHandler<>) //TODO: to a static field
                            .MakeGenericType(eventType)
                            .GetMethod(
                                nameof(IEventHandler<object>.HandleEventAsync),
                                new[] { eventType }
                            );

                        await (Task)method.Invoke(eventHandlerWrapper.EventHandler, new[] { eventData });
                    }
                    else if (ReflectionHelper.IsAssignableToGenericType(handlerType, typeof(IDistributedEventHandler<>)))
                    {
                        var method = typeof(IDistributedEventHandler<>) //TODO: to a static field
                            .MakeGenericType(eventType)
                            .GetMethod(
                                nameof(IDistributedEventHandler<object>.HandleEventAsync),
                                new[] { eventType }
                            );

                        await (Task)method.Invoke(eventHandlerWrapper.EventHandler, new[] { eventData });
                    }
                    else
                    {
                        throw new Exception("The object instance is not an event handler. Object type: " + handlerType.AssemblyQualifiedName);
                    }
                }
                catch (TargetInvocationException ex)
                {
                    exceptions.Add(ex.InnerException);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        /// <summary>
        /// Class EventTypeWithEventHandlerFactories.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected class EventTypeWithEventHandlerFactories
        {
            /// <summary>
            /// Gets the type of the event.
            /// </summary>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public Type EventType { get; }

            /// <summary>
            /// Gets the event handler factories.
            /// </summary>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public List<IEventHandlerFactory> EventHandlerFactories { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="EventTypeWithEventHandlerFactories" /> class.
            /// </summary>
            /// <param name="eventType">Type of the event.</param>
            /// <param name="eventHandlerFactories">The event handler factories.</param>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }
        }

        // Reference from
        // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
        /// <summary>
        /// Struct SynchronizationContextRemover
        /// </summary>
        /// <seealso cref="System.Runtime.CompilerServices.INotifyCompletion" />
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        protected struct SynchronizationContextRemover : INotifyCompletion
        {
            /// <summary>
            /// Gets a value indicating whether this instance is completed.
            /// </summary>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public bool IsCompleted
            {
                get { return SynchronizationContext.Current == null; }
            }

            /// <summary>
            /// Schedules the continuation action that&amp;#39;s invoked when the instance completes.
            /// </summary>
            /// <param name="continuation">The action to invoke when the operation completes.</param>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public void OnCompleted(Action continuation)
            {
                var prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            /// <returns>SynchronizationContextRemover.</returns>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Gets the result.
            /// </summary>
            /// <remarks>
            /// <para>作者    :xuliangjie</para>
            /// <para>创建时间:2018-12-03</para>
            /// <para>最后更新:xuliangjie</para>
            /// <para>更新时间:2018-12-05</para>
            /// </remarks>
            public void GetResult()
            {
            }
        }
    }
}