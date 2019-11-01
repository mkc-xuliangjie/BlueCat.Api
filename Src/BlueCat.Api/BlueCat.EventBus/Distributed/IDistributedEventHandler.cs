
using System.Threading.Tasks;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Interface IDistributedEventHandler
    /// </summary>
    /// <typeparam name="TEvent">The type of the t event.</typeparam>
    /// <seealso cref="BlueCat.EventBus.IEventHandler" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IDistributedEventHandler<in TEvent> : IEventHandler
    {
        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        Task HandleEventAsync(TEvent eventData);
    }
}