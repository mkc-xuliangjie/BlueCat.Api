

using BlueCat.Extensions;
using System;
using System.Threading.Tasks;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Class NullDistributedEventBus. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.Distributed.IDistributedEventBus" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public sealed class NullDistributedEventBus : IDistributedEventBus
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public static NullDistributedEventBus Instance { get; } = new NullDistributedEventBus();

        /// <summary>
        /// Prevents a default instance of the <see cref="NullDistributedEventBus" /> class from being created.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        private NullDistributedEventBus()
        {
            
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
            return NullDisposable.Instance;
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
            return NullDisposable.Instance;
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
            return NullDisposable.Instance;
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
            return NullDisposable.Instance;
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
            return NullDisposable.Instance;
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
            return NullDisposable.Instance;
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
        public Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class
        {
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }
    }
}
