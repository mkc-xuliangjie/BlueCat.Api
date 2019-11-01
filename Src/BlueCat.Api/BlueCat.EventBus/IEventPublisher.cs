

using System;
using System.Threading.Tasks;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventPublisher
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventPublisher
    {
        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="eventData">Related data for the event</param>
        /// <returns>The task to handle async operation</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        Task PublishAsync<TEvent>(TEvent eventData)
            where TEvent : class;

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="eventData">Related data for the event</param>
        /// <returns>The task to handle async operation</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        Task PublishAsync(Type eventType, object eventData);
    }
}