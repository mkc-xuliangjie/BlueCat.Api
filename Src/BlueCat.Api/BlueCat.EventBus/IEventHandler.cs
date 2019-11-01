

using System.Threading.Tasks;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Undirect base interface for all event handlers.
    /// Implement <see cref="IEventHandler{TEvent}" /> instead of this one.
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventHandler
    {
        
    }

    /// <summary>
    /// Defines an interface of a class that handles events asynchrounously of type <see cref="IEventHandler{TEvent}" />.
    /// </summary>
    /// <typeparam name="TEvent">Event type to handle</typeparam>
    /// <seealso cref="BlueCat.EventBus.IEventHandler" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventHandler<in TEvent> : IEventHandler
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