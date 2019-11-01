

using System;
using System.Threading.Tasks;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventPublisher
    /// </summary>
    /// <remarks>
    /// <para>����    :xuliangjie</para>	
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>	
    /// <para>����ʱ��:2018-12-05</para>
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
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
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
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        Task PublishAsync(Type eventType, object eventData);
    }
}